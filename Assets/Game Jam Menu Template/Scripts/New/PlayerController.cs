using UnityEngine;
using System.Collections;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour {

	public delegate void HandleInstancePlayer();
	public HandleInstancePlayer OnPlayerInstance;

	public delegate void MyDelegate();
	public MyDelegate OnSwitchWeapon, OnShoot;

	public delegate void KillEnemy(int score);
	public KillEnemy OnKillEnemy;

	public delegate void KillHostageHandler();
	public static event KillHostageHandler OnKillHostage;

	public int joystickNumber = 1;
	public int playerNumber = 1;

	public LayerMask ignoreLayer;

	public Transform crosshairSprite;
	public float crosshairSpeed = 12;
	public float crosshairAcceleration = 30;

	public float playerSpeed;

	public GameObject shell;
	public Transform shellPoint;

	public int score;

	private float damagePerShot;
	private float fireRate;

	private float timer; 


	public WeaponObject[] weapon;
	public int currentWeapon = 0;

	float _currentSpeedCrosshairX;
	float _currentSpeedCrosshairY;
	float _currentSpeedCharH;
	float  _currentPosCrosshairX;
	float  _currentPosCrosshairY;
	Vector3 amountToMoveCrosshair;
	Vector3 amountToMovePlayer;

	Vector3 upperCorner;
	float maxWidth;

	float distanceAimToChar;

	CharacterController _characterController;
	Animator anim;

	protected float playerSpeedH;

	public AudioClip shoot;

	Ray shootRay;
	RaycastHit shootHit;

	#region FSM

	public FSM	Fsm{get;private set;}

	public IdleState 	idleState;

	public MovingState 	movingState;

	#endregion


	void Awake () 
	{
		ignoreLayer = ~ignoreLayer;

		anim = GetComponent<Animator>();

		Cursor.visible = false;

		Fsm = FSM.CreateFSM(this);
		Fsm.AddFSMState (idleState, movingState);

		Fsm.ChangeState(idleState);


		transform.parent.gameObject.SetActive(false);
	
	}


	public void Init(int _joystickNumber)
	{
		joystickNumber = _joystickNumber;
		transform.parent.gameObject.SetActive(true);
		anim.SetTrigger("Blink");

		InitWeapons();

		Camera.main.GetComponent<ProCamera2D>().AddCameraTarget(transform,.5f);

		if(OnPlayerInstance != null)
			OnPlayerInstance();

	}

	private void InitWeapons()
	{
		for(int i=0; i <= weapon.Length -1; i++)
		{
			weapon[i].currentBullets = weapon[i].maxBullets;
		}
	}

	void PlayerMovementClamping()
	{
		var viewpointCoord = Camera.main.WorldToViewportPoint(crosshairSprite.transform.position);
		viewpointCoord.x = Mathf.Clamp01(viewpointCoord.x);
		viewpointCoord.y = Mathf.Clamp01(viewpointCoord.y);
		crosshairSprite.transform.position 	= Camera.main.ViewportToWorldPoint(viewpointCoord);
		var limitPlayer = transform.position;
		limitPlayer.x = Mathf.Clamp(transform.position.x, Camera.main.GetComponent<ProCamera2DNumericBoundaries>().LeftBoundary, Camera.main.GetComponent<ProCamera2DNumericBoundaries>().RightBoundary);
		transform.position = limitPlayer;
	}

	void LateUpdate()
	{
		PlayerMovementClamping();
	}

	void Update()
	{
		//If we are playing with keyboard
		if(joystickNumber == 3)
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			crosshairSprite.transform.position = new Vector3(pos.x,pos.y,0);
		}
		else
		{

			//Crosshair speed X
			var crosshairSpeedX = Input.GetAxis("XCrossPlayer"+joystickNumber) * crosshairSpeed;
			_currentSpeedCrosshairX = IncrementTowards(_currentSpeedCrosshairX, crosshairSpeedX, crosshairAcceleration);

			//Crosshair speed Y
			var crosshairSpeedY = Input.GetAxis("YCrossPlayer"+joystickNumber) * crosshairSpeed;
			_currentSpeedCrosshairY = IncrementTowards(_currentSpeedCrosshairY, crosshairSpeedY, crosshairAcceleration);

			//Amount to move crosshair (This is for acceleration)
			amountToMoveCrosshair.x = _currentSpeedCrosshairX;
			amountToMoveCrosshair.y = _currentSpeedCrosshairY;

			//Move the crosshair
			crosshairSprite.Translate(amountToMoveCrosshair * Time.deltaTime);


		}

		//Current Crosshair position 
		_currentPosCrosshairX = crosshairSprite.position.x;
		_currentPosCrosshairY = crosshairSprite.position.y;

		//Movement velocity of character
		playerSpeedH = Input.GetAxis("XMovePlayer" + joystickNumber) * playerSpeed;

		//Distance between our character and crosshair
		distanceAimToChar = _currentPosCrosshairX - transform.position.x;

		//Move the character
		transform.Translate(amountToMovePlayer * Time.deltaTime);

		//FIRE!!!
		timer += Time.deltaTime;

		fireRate = weapon[currentWeapon].fireRate;


		//I only can fire if i'm stopped
		if(_currentSpeedCharH == 0)
		{
			//Weapon automatic
			if(weapon[currentWeapon].automatic)
			{
				if(Input.GetButton ("FirePlayer"+joystickNumber))
				{
					Time.timeScale = 1.0F;
					if (timer >= fireRate && Time.timeScale != 0)
					{
						Shoot ();
					}
				}
			}
			//Weapon no automatic
			else
			{
				if(Input.GetButtonDown ("FirePlayer"+joystickNumber))
				{
					Time.timeScale = 1.0F;
					if (timer >= fireRate && Time.timeScale != 0)
					{
						Shoot ();
					}
				}
			}
		}

		if(Input.GetButtonDown("TrianglePlayer"+joystickNumber))
		{
			SwitchWeapon();


			if(OnSwitchWeapon != null)
			{
				OnSwitchWeapon();
			}
		}


	}

	void SwitchWeapon()
	{
		currentWeapon++;
		if(currentWeapon == weapon.Length)
		{
			currentWeapon = 0;
		}
	}

	void Shoot ()
	{
		timer = 0f;
		
		if(OnShoot != null)
			OnShoot();
		
		if(weapon[currentWeapon].currentBullets <= 0 && !weapon[currentWeapon].infiniteBullets)
		{
			//Sonido de no bullets
			return;
		}

		Instantiate(shell, shellPoint.position, Quaternion.identity);
		anim.SetTrigger("Shoot");
		SoundManager.Instance.Play2DSound(shoot);
		GameManager.instance.Flash();

		RaycastHit2D hit = Physics2D.Raycast(crosshairSprite.position, Vector2.zero, ignoreLayer );
		if(hit.collider != null)
		{
			if(hit.collider.tag == "Enemy")
			{
				LifeModule life = hit.collider.transform.parent.GetComponent<LifeModule>();

				if(hit.collider.name == "Head")
				{
					life.DoDamage(weapon[currentWeapon].damage * 3);
					Instantiate(Resources.Load("blood"),hit.point,Quaternion.identity);
				}
				else
				{
					life.DoDamage(weapon[currentWeapon].damage);
				}

				//If i kill an enemy
				if(life.currentLife <= 0)
				{
					if(OnKillEnemy != null)
					{
						OnKillEnemy(life.GetComponent<EnemyController>().scoreToKill);
						GameManager.instance.currentZombiesKilled++;

						if(GameManager.instance.currentZombiesKilled >= SpawnerEnemies.instance.CurrentWave.count)
						{
							GameManager.instance.currentZombiesKilled = 0;
							GameManager.instance.NextWave();
						}
					}
				}
			}
			else if(hit.collider.tag == "Fore")
			{
				Instantiate(Resources.Load("Decal"),hit.point,Quaternion.identity);
			}
			else if(hit.collider.tag == "Boss")
			{
				SceneManager.LoadScene("Win");
			}
		}

		//rest bullets
		if(!weapon[currentWeapon].infiniteBullets)
		{
			weapon[currentWeapon].currentBullets--;
		}
	}

	private void Flip(int num)
	{
		Vector3 theScale = transform.localScale;
		theScale.x = num;
		transform.localScale = theScale;
	}

	// Increase n towards target by speed
	private float IncrementTowards(float n, float target, float a)
	{
		if (n == target)
		{
			return n;   
		}
		else
		{
			float dir = Mathf.Sign(target - n); 
			n += a * Time.deltaTime * dir;
			return (dir == Mathf.Sign(target - n)) ? n : target;
		}
	}


	[System.Serializable]
	public class IdleState : FSM.FSMState
	{
		private PlayerController myOwner;

		public override void Init ()
		{
			base.Init ();

			myOwner = owner as PlayerController;

		}

		public override void Enter (Hashtable _parameters)
		{
			base.Enter (_parameters);

			myOwner.anim.SetBool("Idle",true);
		}

		public override void Update ()
		{
			base.Update ();

			//make flip 
			if((int)Mathf.Clamp(myOwner.distanceAimToChar,-1,1) != 0)
				myOwner.Flip((int)Mathf.Clamp(myOwner.distanceAimToChar,-1,1));

			myOwner.anim.SetFloat("AimDistance",Mathf.Clamp01(Mathf.Abs(myOwner.distanceAimToChar)/10));



			if(myOwner.playerSpeedH != 0)
			{
				ChangeState(myOwner.movingState);
			}

		}

		public override void Exit (FSM.FSMState _nextState)
		{
			myOwner.anim.SetBool("Idle",false);
		}

	}

	[System.Serializable]
	public class MovingState : FSM.FSMState
	{
		private PlayerController myOwner;
		public override void Init ()
		{
			base.Init ();

			myOwner = owner as PlayerController;

		}

		public override void Enter (Hashtable _parameters)
		{
			base.Enter (_parameters);


		}

		public override void Update ()
		{
			base.Update ();



			myOwner._currentSpeedCharH = myOwner.playerSpeedH * 100 * Time.deltaTime;//myOwner.IncrementTowards(myOwner._currentSpeedCharH, myOwner.playerSpeedH, 0);

			myOwner.amountToMovePlayer.x = myOwner._currentSpeedCharH;

			if(myOwner.playerSpeedH != 0)
			{
				myOwner.anim.SetBool("Run", true);
				myOwner.anim.SetFloat("Speed", Mathf.Abs( myOwner.playerSpeedH/myOwner.playerSpeed));
				if(myOwner.playerSpeedH < 0)
					myOwner.Flip(-1);
				else
					myOwner.Flip(1);

			}
			else
			{
				myOwner.anim.SetBool("Run", false);
				ChangeState(myOwner.idleState);
			}
				
		}

		public override void Exit (FSM.FSMState _nextState)
		{

		}

	}
}
