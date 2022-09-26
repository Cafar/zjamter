using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class Aim : MonoBehaviour {

	public LayerMask ignoreLayer;

	
	public float rechargeCouldown;
	public float shakeCouldown;
	

	public int totalBullets;
	public int currentBullets;
	
	public float amplitudeX = 10.0f;
	public float amplitudeY = 5.0f;
	public float omegaX = 1.0f;
	public float omegaY = 5.0f;


	public float nervious = 1;

	public Text numBullets;



	public AudioClip fxShot, fxReload;

	private Vector3 axis;

	private CircleCollider2D col;
	private Vector3 pos;
	private float time;
	private bool shaking;

	#region FSM
	
	public FSM	Fsm{get;private set;}
	
	public AimState 	aimState;
	
	public ShotState 	shotState;
	
	public ReloadState	reloadState;

	public DeadState	deadState;
	
	
	#endregion


	// Use this for initialization
	void Start () 
	{
		time = shakeCouldown;
		col = GetComponent<CircleCollider2D>();
		col.enabled = false;
		shaking = false;
		Cursor.visible = false;
		currentBullets = totalBullets;
		Screen.SetResolution(720,480,true);
		ignoreLayer = ~ignoreLayer;

		pos = transform.position;
		axis = transform.up;  // May or may not be the axis you want

		Fsm = FSM.CreateFSM(this);
		Fsm.AddFSMState (aimState, shotState, reloadState,deadState);

		Fsm.ChangeState(aimState);



		numBullets.text = "Bullets: "+  currentBullets.ToString();
	}

	void OnTriggerEnter2D(Collider2D col)
	{


	}

	[System.Serializable]
	public class AimState : FSM.FSMState
	{
		private Aim myOwner;
		private Vector3 pos;
		private float index;
		public override void Init ()
		{
			base.Init ();
			
			myOwner = owner as Aim;
			
		}
		
		public override void Enter (Hashtable _parameters)
		{
			base.Enter (_parameters);
			
			pos = myOwner.pos;

			if(myOwner.currentBullets <= 0)
			{
				myOwner.numBullets.text = "Reload with R";
			}
			else
			{
				myOwner.numBullets.text = "Bullets: "+  myOwner.currentBullets.ToString();
			}
		}
		
		public override void Update ()
		{
			base.Update ();

			pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			pos.x = Mathf.Clamp(pos.x, -23.5f, 23.5f);
			pos.y = Mathf.Clamp(pos.y, -7f, 5f);

			index += Time.deltaTime;
			float x = myOwner.amplitudeX*Mathf.Cos (myOwner.omegaX*index);
			float y = Mathf.Abs (myOwner.amplitudeY*Mathf.Sin (myOwner.omegaY*index));
			Vector3 nervios = new Vector3(x,y,-10);
			myOwner.transform.position =  Vector3.Lerp(myOwner.transform.position, pos + (nervios * myOwner.nervious) , Time.deltaTime);
			myOwner.transform.position = new Vector3 (myOwner.transform.position.x, myOwner.transform.position.y, -10);

			//pos += Random.insideUnitSphere * Time.deltaTime * myOwner.MoveSpeed;
			//myOwner.transform.position = pos + myOwner.axis * Mathf.Sin (Time.time * myOwner.frequency) * myOwner.magnitude;

			
			if(Input.GetButtonDown("Fire1") && myOwner.currentBullets >= 1)
			{
				ChangeState(myOwner.shotState);
			}

			if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.R))
			{
				ChangeState(myOwner.reloadState);
			}
		}
		
		public override void Exit (FSM.FSMState _nextState)
		{
			
		}
		
	}

	[System.Serializable]
	public class ShotState : FSM.FSMState
	{
		private Aim myOwner;
		private float time;
	

		public override void Init ()
		{
			base.Init ();
			
			myOwner = owner as Aim;
			
		}
		
		public override void Enter (Hashtable _parameters)
		{
			base.Enter (_parameters);

			time = 0;
			myOwner.col.enabled = true;
			RaycastHit2D hit = Physics2D.Raycast(myOwner.transform.position, Vector2.zero, myOwner.ignoreLayer );
			if(hit.collider != null)
			{
//				Debug.Log(hit.collider.name);
				if(hit.collider.tag == "Enemy")
				{
					if(hit.collider.name == "Head")
					{
						hit.collider.transform.parent.GetComponent<LifeModule>().DoDamage(3);
					}
					else
					{
						hit.collider.transform.parent.GetComponent<LifeModule>().DoDamage(1);

					}
				}
			}
			myOwner.currentBullets--;
			myOwner.numBullets.text = "Bullets: "+ myOwner.currentBullets.ToString();
			myOwner.transform.DOShakePosition(1);
			SoundManager.Instance.Play2DSound(myOwner.fxShot,1,0,.3f);
		}
		
		public override void Update ()
		{
			base.Update ();


			time += Time.deltaTime;

			//chapuzilla
			if(time >= 0.01f)
			{
				myOwner.col.enabled = false;
			}

			if(time >= myOwner.shakeCouldown)
			{
				ChangeState(myOwner.aimState);
			}
		}
		
		public override void Exit (FSM.FSMState _nextState)
		{
			
		}
		
	}

	[System.Serializable]
	public class ReloadState : FSM.FSMState
	{
		private Aim myOwner;
		private float time;
		public override void Init ()
		{
			base.Init ();
			
			myOwner = owner as Aim;
			
		}
		
		public override void Enter (Hashtable _parameters)
		{
			base.Enter (_parameters);
			
			myOwner.currentBullets = myOwner.totalBullets;
			myOwner.numBullets.text = "Bullets: "+ myOwner.currentBullets.ToString();
			time = 0;
			myOwner.transform.DOShakePosition(1,.3f,3);
			SoundManager.Instance.Play2DSound(myOwner.fxReload);
		}
		
		public override void Update ()
		{
			base.Update ();

			time += Time.deltaTime;

			if(time >= myOwner.rechargeCouldown)
			{
				ChangeState(myOwner.aimState);
			}

		}
		
		public override void Exit (FSM.FSMState _nextState)
		{
			
		}
		
	}

	[System.Serializable]
	public class DeadState : FSM.FSMState
	{
		private Aim myOwner;
		private float time;
		public override void Init ()
		{
			base.Init ();
			
			myOwner = owner as Aim;
			
		}
		
		public override void Enter (Hashtable _parameters)
		{
			base.Enter (_parameters);

			Vector3 pos = (Vector3) _parameters["pos"];
			pos.z = -10;
			myOwner.transform.DOMove (pos,1);
		}
		
		public override void Update ()
		{
			base.Update ();
			
			time += Time.deltaTime;

			//CUando pierdo espero 2 segundos
			if(time >= 3)
			{
				Application.LoadLevel("Finish");
			}
			
		}
		
		public override void Exit (FSM.FSMState _nextState)
		{
			
		}
		
	}
}
