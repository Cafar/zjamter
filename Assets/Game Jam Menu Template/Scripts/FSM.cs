using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Finite state machine based on this one http://www.playmedusa.com/blog/2010/12/10/a-finite-state-machine-in-c-for-unity3d/
public class FSM : MonoBehaviour
{
	
	public Object 		Owner { get; set; }
	
	[HideInInspector]
	public string
		fsmName = "Finite state machine";
	
	public delegate void ChangeStateHandler(FSMState _oldState,FSMState _newState);
	public event ChangeStateHandler OnStateChange;

	public bool 		logChanges = false;
	
	public  FSMState	initialState;
	
	private FSMState 	CurrentState;
	private FSMState 	PreviousState;

	private List<FSMState>	StatesRecord = new List<FSMState>();

	/// <summary>
	/// Only filled the frame we do a state change
	/// </summary>
	private FSMState 	NextStateVolatile;
	
	/// <summary>
	/// Save time in current state
	/// </summary>
	private float		timeInCurrentState;
	
	/// <summary>
	/// Store states added to this fsm state;
	/// </summary>
	private List<FSMState>	states = new List<FSMState>();
	
	public class FSMState
	{	
		[HideInInspector]
		public Object
			owner;	
		
		[HideInInspector]
		public FSM
			fsm;
		
		[HideInInspector]
		public List<FSM>
			subFSMList;
		
		/// <summary>
		/// The initial state. Only one 
		/// </summary>
		public bool		initialState;
		
		/// <summary>
		/// We can change to this state from this state
		/// </summary>
		public 	bool	recursiveState;
		
		/// <summary>
		/// When we enter in this state, we can not change to another state
		/// </summary>
		public	bool	finalState;
	
		
		protected void ChangeState(FSMState _newState, Hashtable _parameters = null)
		{
			fsm.ChangeState(_newState, _parameters);
		}
		
		public virtual void Enter(Hashtable _parameters = null)
		{
			if(!fsm.states.Contains(this))
				Debug.LogError(string.Format("[ERROR] You must add the current state: {0}", this.ToString()));
			
			EnableSubFSM();
		}
		
		public FSM CreateSubFSM(MonoBehaviour _owner)
		{
			if(subFSMList == null)
				subFSMList = new List<FSM>();
			
			FSM subFSM = _owner.gameObject.AddComponent(typeof(FSM)) as FSM;
			subFSM.ResetFSM(_owner);
			subFSM.Owner = _owner;
			subFSMList.Add(subFSM);
			return subFSM;
		}
		
		/// <summary>
		/// When we enter in this state, enable each sub machine state with the initial state
		/// </summary>
		private void EnableSubFSM()
		{
			for(int i = 0; i<subFSMList.Count; i++)
			{
				subFSMList[i].enabled = true;
			}
		}
		
		public virtual void Init()
		{
		}
		public virtual void Update()
		{
		}
		public virtual void FixedUpdate()
		{
		}
		public virtual void LateUpdate()
		{
		}
		public virtual void OnGUI()
		{
		}
		
		public virtual void Exit(FSM.FSMState _nextState)
		{
			//Disable all submachine states
			for(int i = 0; i<subFSMList.Count; i++)
			{
				if(subFSMList[i].CurrentState != null)
					subFSMList[i].CurrentState.Exit(_nextState);

				subFSMList[i].ResetFSM(false);
				subFSMList[i].enabled = false;	
			}
		}
		
		public virtual void MyReset(bool _nextLevelReset)
		{
			foreach(FSM subFsm in subFSMList)
				subFsm.ResetFSM(_nextLevelReset);
		}
	}
	
	public void AddFSMState(FSMState _existingState)
	{
		_existingState.owner = Owner;
		_existingState.fsm = this;
		_existingState.Init();
		states.Add(_existingState);
	}

	public void AddFSMState(params FSMState[] _existingStates)
	{
		foreach(FSMState iteratorState in _existingStates)
		{
			iteratorState.owner = Owner;
			iteratorState.fsm = this;
			iteratorState.Init();
			states.Add(iteratorState);
		}
	}
	
	public static FSM CreateFSM(MonoBehaviour _owner)
	{
		FSM fsm = _owner.gameObject.AddComponent(typeof(FSM)) as FSM;
		
		fsm.ResetFSM(false);
		fsm.Owner = _owner;
		
		return fsm;
	}

	void Start()
	{
		foreach(FSM.FSMState state in states)
		{
			if(state.initialState)
			{
				ChangeState(state, new Hashtable() { { "isFirstTime", true}  });
				break;
			}
		}
	}
	
	public void  Update()
	{
		if(CurrentState != null) 
			CurrentState.Update();
		
		NextStateVolatile = null;
		
		timeInCurrentState += Time.deltaTime / Time.timeScale;
	}
	
	public void  FixedUpdate()
	{
		if(CurrentState != null) 
			CurrentState.FixedUpdate();

		NextStateVolatile = null;
	}
	
	private void LateUpdate()
	{
		if(CurrentState != null) 
			CurrentState.LateUpdate();
	}

	private void OnGUI()
	{
		if(CurrentState != null) 
			CurrentState.OnGUI();
	}
	
	public void ChangeState(FSMState NewState, Hashtable _parameters = null)
	{
		if(CurrentState != null && ((CurrentState == NewState && !CurrentState.recursiveState) || CurrentState.finalState))
			// (Avoid enter in a state currently active and non recursive) || (Not change state if we are in a final state)
			return;

		if(!GetStates().Contains(NewState))
		{
			Debug.LogError(NewState + " isn't containe in the current FSM: " + name + " add it before changing state");
		}

		bool wasInRecord = false;

		for(int i = 0; i < StatesRecord.Count; ++i)
		{
			if(StatesRecord[i] == NewState)
			{
				wasInRecord = true;
				StatesRecord.RemoveRange(i + 1, StatesRecord.Count - (i + 1));
				break;
			}
		}
		if(!wasInRecord)
			StatesRecord.Add(NewState);
		
		timeInCurrentState = 0;
		PreviousState = CurrentState;

		if(logChanges)
		{
			Debug.Log(string.Format("Fsm: {0} To: {1} From {2}", Owner.ToString(), NewState, PreviousState));
		}

		bool isSubState = IsSubState(NewState);
		
		if(CurrentState != null && !isSubState)
			CurrentState.Exit(NewState);
		
		CurrentState = NewState;
		
		if(CurrentState != null)
			CurrentState.Enter(_parameters);
		
		// Trigger event !!
		if(OnStateChange != null)
			OnStateChange(PreviousState, CurrentState);
		
		NextStateVolatile = NewState;
	}

	public void ChangeToPreviousStateInRecord(Hashtable _parameters = null)
	{
		ChangeState(StatesRecord[StatesRecord.Count - 2], _parameters);
	}

	public void  RevertToPreviousState()
	{
		if(PreviousState != null)
			ChangeState(PreviousState);
	}
	
	public FSMState GetCurrentState()
	{
		return CurrentState;
	}
	
	public int GetCurrentStateIndex()
	{
		return states.IndexOf(CurrentState);
	}
	
	public FSM.FSMState GetStateFromIndex(int _index)
	{
		if(_index >= states.Count)
			Debug.LogError("_index : " + _index + " out of range for this state machine with: " + states.Count + " states in: " + name);
		
		return states[_index];
	}
	
	public FSMState GetPreviousState()
	{
		return PreviousState;
	}
	
	/// <summary>
	/// Resets the FSM CurrentState = null.
	/// </summary>
	public void ResetFSM(bool _nextLevelReset)
	{
		PreviousState = null;
		CurrentState = null;
		NextStateVolatile = null;
		
		timeInCurrentState = 0f;
		
		// Reset all states
		foreach(FSMState state in states)
		{
			state.MyReset(_nextLevelReset);	
		}
	}
	
	/// <summary>
	/// True when a state change has been done this frame.
	/// </summary>
	public bool IsGoingToChangeStateAtFrame()
	{
		return NextStateVolatile != null;
	}
	
	/// <summary>
	/// Until ChangeState is called by first time, this DSM can has not any state setted.
	/// </summary>
	public bool HasState()
	{
		return CurrentState != null;	
	}
	
	/// <summary>
	/// Time in seconds we are in this state
	/// </summary>
	public float GetTimeInCurrentState()
	{
		return timeInCurrentState;
	}
	
	/// <summary>
	/// Gets the states of this state machine
	/// </summary>
	public List<FSM.FSMState> GetStates()
	{
		return states;	
	}

	/// <summary>
	/// Determines if the given state is a substate of the current state
	/// </summary>
	private bool IsSubState(FSM.FSMState _newState)
	{
		bool isSubState = false;

		if(CurrentState == null)
			return isSubState;

		// TODO: This method must be recursive to support multipble sublevels of FSMs
		foreach(var subFsm in CurrentState.subFSMList)
		{
			if(subFsm.GetStates().Contains(_newState))
			{
				isSubState = true;
				break;
			}
		}

		return isSubState;
	}
}
