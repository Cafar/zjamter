using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour {

//	public delegate void HandleOnStartGame();
//	public static event HandleOnStartGame OnStartGame;

//	public delegate void HandleOnWaveClear();
//	public static event HandleOnWaveClear OnWaveClear;

	public static GameManager instance;

	public SpawnerEnemies gameSpawner;

	public int m_currentWaveIndex;
	public int currentZombiesKilled;

	public AudioClip[] soundsVictims;

	public Image flash;

	public GameObject Instructions, finishGameobject;

	public Text wave;

	private Animator animFlash;

	//Number of players playing
	private int NumberPlayerToPressStart;

	private bool controller1Activate, controller2Activate, controller3Activate; //3 is keyboard

	public Transform player1, player2;

	private PlayerController player1Controller, player2Controller;

	private int joystickNumber;

	private bool isFinishGame;

	#region FSM

	public FSM	Fsm{get;private set;}

	public StartState 		startState;

	public PlayingState 	playingState;

	public LoseState		loseState;



	#endregion

	void Awake () 
	{
		instance = this;

		PlayerController.OnKillHostage += PlayerController_OnKillHostage;

		m_currentWaveIndex = 0;

		controller1Activate = controller2Activate = false;

		NumberPlayerToPressStart = 1;

		animFlash = flash.GetComponent<Animator>();

		Fsm = FSM.CreateFSM(this);
		Fsm.AddFSMState (startState, playingState, loseState);

		Fsm.ChangeState(startState);
	}

	void Start()
	{
		player1Controller = player1.GetComponentInChildren<PlayerController>();
		player2Controller = player2.GetComponentInChildren<PlayerController>();
	}

	//When we kill some hostage
	void PlayerController_OnKillHostage ()
	{
		
	}

	public void Flash()
	{
		animFlash.SetTrigger("flash");
	}

	public void NextWave ()
	{
		StartCoroutine( WaveTextAnim());
	}

	IEnumerator WaveTextAnim()
	{
		m_currentWaveIndex++;
		if(m_currentWaveIndex == SpawnerEnemies.instance.m_waves.Count +1)
		{
			wave.text = "BOSS";
		}
		else
		{
			wave.text = "WAVE "+ m_currentWaveIndex +"/"+10;
		}
		wave.transform.DOScale(Vector3.one,2);
		yield return new WaitForSeconds(2);
		wave.transform.DOScale(Vector3.zero,1);
		SpawnerEnemies.instance.NextWave();
	}

	void Update () 
	{
		if(Input.GetButtonDown ("StartPlayer1"))
		{
			if(!controller1Activate)
			{
				joystickNumber = 1;
				controller1Activate = true;
				InitPlayer(joystickNumber);
			}
			else
			{
				//Puase
			}
		}
		if(Input.GetButtonDown ("StartPlayer2"))
		{
			if(!controller2Activate)
			{
				joystickNumber = 2;
				controller2Activate = true;
				InitPlayer(joystickNumber);
			}
			else
			{
				//Puase
			}
		}
		if(Input.GetButtonDown ("StartPlayer3"))
		{
			if(!controller3Activate)
			{
				joystickNumber = 3;
				controller3Activate = true;
				InitPlayer(joystickNumber);
			}
			else
			{
				//Puase
			}
		}
	}

	void InitPlayer(int _joystickNumber)
	{
		if(NumberPlayerToPressStart == 1)
		{
			player1Controller.Init(_joystickNumber);
			StartCoroutine("VictimasSonidos");
			StartGame();
		}
		else if(NumberPlayerToPressStart == 2)
		{
			player2Controller.Init(_joystickNumber);
		}
		NumberPlayerToPressStart++;



	}

	IEnumerator VictimasSonidos()
	{

		SoundManager.Instance.Play2DSound(soundsVictims[Random.Range(0,soundsVictims.Length)]);
		yield return new WaitForSeconds(Random.Range(3,10));
		StartCoroutine("VictimasSonidos");
	}

	void StartGame()
	{
		Fsm.ChangeState(playingState);
	}

	void OnDead()
	{
		

	}

	[System.Serializable]
	public class StartState : FSM.FSMState
	{

		private GameManager myOwner;

		public override void Init ()
		{
			base.Init ();

			myOwner = owner as GameManager;

		}

		public override void Enter (Hashtable _parameters)
		{
			base.Enter (_parameters);


		}

		public override void Update ()
		{
			base.Update ();

		}

		public override void Exit (FSM.FSMState _nextState)
		{
			base.Exit (_nextState);



		}

		private void Attack()
		{

		}

	}

	[System.Serializable]
	public class PlayingState : FSM.FSMState
	{

		private GameManager myOwner;

		public override void Init ()
		{
			base.Init ();

			myOwner = owner as GameManager;

		}

		public override void Enter (Hashtable _parameters)
		{
			base.Enter (_parameters);

			myOwner.NextWave();
			foreach(Image im in myOwner.Instructions.GetComponentsInChildren<Image>())
				im.DOFade(0,1);
			foreach(Text tx in myOwner.Instructions.GetComponentsInChildren<Text>())
				tx.DOFade(0,1);
		}

		public override void Update ()
		{
			base.Update ();

		}

		public override void Exit (FSM.FSMState _nextState)
		{
			base.Exit (_nextState);



		}

		private void Attack()
		{

		}

	}
	[System.Serializable]
	public class LoseState : FSM.FSMState
	{

		private GameManager myOwner;

		public override void Init ()
		{
			base.Init ();

			myOwner = owner as GameManager;

		}

		public override void Enter (Hashtable _parameters)
		{
			base.Enter (_parameters);


		}

		public override void Update ()
		{
			base.Update ();

		}

		public override void Exit (FSM.FSMState _nextState)
		{
			base.Exit (_nextState);



		}

		private void Attack()
		{

		}

	}
}
