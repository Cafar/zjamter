using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour {
	
	public float attackVelocity = 1;
	public float damage = 1;
	public float velocity = 1;
	public int scoreToKill = 10;

	public ParticleSystem particleDead;
	
	public List<AudioClip> fxWalk, fxHit, fxDead;
	public AudioClip mastica;
	private Animator animator;
	private LifeModule lifeModule;

	private AudioSource audiosource;

	#region FSM
	
	public FSM	Fsm{get;private set;}
	
	public MovingState 		movingState;
	
	public FightingState 	fightingState;

	public HitState		 	hitState;
		
	public DeathState 		deathState;
	
	
	
	#endregion

	void Awake()
	{
		animator = GetComponent<Animator>();
		audiosource = GetComponent<AudioSource>();
	}


	void Start()
	{


		//if(Random.Range(0,100) >= 95)
		//	velocity *= 2;
		//Initialize FSM
		Fsm = FSM.CreateFSM(this);
		Fsm.AddFSMState (fightingState, movingState, hitState,deathState);
		
		Fsm.ChangeState(movingState);

		lifeModule = GetComponent<LifeModule>();
		lifeModule.OnKill += HandleOnKill;
		lifeModule.OnLifeChange += HandleOnLifeChange;

		audiosource.PlayOneShot(fxWalk[Random.Range(0,fxWalk.Count)]);

		Instantiate(Resources.Load("Flash"),transform.position,Quaternion.identity);


	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.transform.tag == "Victim")
		{
			foreach(EnemyController tr in FindObjectsOfType(typeof(EnemyController)))
			{
				tr.velocity = 0;
			}
			SceneManager.LoadScene("Finish");
		}

	}

	void HandleOnLifeChange (LifeModule _who, float _currentLife, float _previous, float _percentage)
	{
		if(_currentLife > 0)
			Fsm.ChangeState(hitState);
	}

	
	void HandleOnKill (GameObject _who)
	{
		Fsm.ChangeState(deathState);
		PlayerPrefs.SetInt("CivilsSaved", PlayerPrefs.GetInt("CivilsSaved") + 1);
	}

	void Update()
	{
		
	}

	#region states
	
	
	[System.Serializable]
	public class MovingState : FSM.FSMState
	{
		private EnemyController myOwner;
		private float time;
		private float randomNum;
		public override void Init ()
		{
			base.Init ();
			
			myOwner = owner as EnemyController;
			
		}
		
		public override void Enter (Hashtable _parameters)
		{
			base.Enter (_parameters);

			myOwner.animator.SetTrigger("Walk");

			randomNum = Random.Range(10,17);
		}
		
		public override void Update ()
		{
			base.Update ();
			
			myOwner.GetComponent<Rigidbody2D>().velocity =  new Vector2(-myOwner.velocity, myOwner.GetComponent<Rigidbody2D>().velocity.y);

			time += Time.deltaTime;
			
			//chapuzilla
			if(time >= randomNum)
			{
				myOwner.audiosource.PlayOneShot(myOwner.fxWalk[Random.Range(0,myOwner.fxWalk.Count)]);
				time = 0;
			}

			//float distance = Vector2.Distance(myOwner.transform.position, myOwner.mainCharacter.transform.position);
			
//			if(distance < 1F)
//			{
//				myOwner.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
//				ChangeState(myOwner.fightingState);
//			}
			//Debug.Log(distance);
		}
		
		public override void Exit (FSM.FSMState _nextState)
		{
			
		}
		
	}
	
	[System.Serializable]
	public class FightingState : FSM.FSMState
	{
		
		private EnemyController myOwner;

		public override void Init ()
		{
			base.Init ();
			
			myOwner = owner as EnemyController;
			
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
	public class HitState : FSM.FSMState
	{
		
		private EnemyController myOwner;

		public override void Init ()
		{
			base.Init ();
			
			myOwner = owner as EnemyController;
			
		}
		
		public override void Enter (Hashtable _parameters)
		{
			base.Enter (_parameters);
			
			myOwner.animator.SetTrigger("Blink");
			myOwner.audiosource.PlayOneShot(myOwner.fxHit[(int)Random.Range(0,myOwner.fxHit.Count)]);
			ChangeState(myOwner.movingState);
		}
		
		public override void Update ()
		{
			base.Update ();
			

		}
		
		public override void Exit (FSM.FSMState _nextState)
		{
			base.Exit (_nextState);
			
		
			
		}
		
	}

	[System.Serializable]
	public class DeathState : FSM.FSMState
	{
		
		private EnemyController myOwner;
		
		public override void Init ()
		{
			base.Init ();
			
			myOwner = owner as EnemyController;
		}
		
		public override void Enter (Hashtable _parameters)
		{
			base.Enter (_parameters);
			myOwner.audiosource.PlayOneShot(myOwner.fxDead[(int)Random.Range(0,myOwner.fxDead.Count)]);

			myOwner.GetComponentInChildren<BoxCollider2D>().enabled = false;
			foreach(CircleCollider2D cir in myOwner.GetComponentsInChildren<CircleCollider2D>())
			{
				if(cir.name == "Head")
					cir.enabled = false;
			}

			myOwner.animator.SetTrigger("Dead");
			Destroy(myOwner.gameObject,1);
		}
		
	}
	
	
	
	
	#endregion
}
