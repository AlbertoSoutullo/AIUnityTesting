public class FiniteStateMachine <T>  {
	private T Owner;
	private FSMState<T> currentState;
	private FSMState<T> previousState;
	private FSMState<T> globalState;

	public FiniteStateMachine(T owner)
	{
		this.Owner = owner;
		this.currentState = null;
		this.previousState = null;
		this.globalState = null;
	}

	public void Update()
	{
		if (globalState != null) globalState.Execute(Owner);
		if (currentState != null) currentState.Execute(Owner);
	}

	public void ChangeState(FSMState<T> newState)
	{
		PreviousState = currentState;
		if (currentState != null) currentState.Exit(Owner);
		currentState = newState;
		if (currentState != null) currentState.Enter(Owner);
	}

	public FSMState<T> CurrentState { get { return currentState; } set { currentState = value; } }
	public FSMState<T> PreviousState { get { return previousState; } set { previousState = value; } }
	public FSMState<T> GlobalState { get { return globalState; } set { globalState = value; } }

	//public void Awake()
	//{		
	//	CurrentState = null;
	//	PreviousState = null;
	//	GlobalState = null;
	//}

	//public void Configure(T owner, FSMState<T> InitialState) {
	//	Owner = owner;
	//	ChangeState(InitialState);
	//}

	//public void  RevertToPreviousState()
	//{
	//	if (PreviousState != null) ChangeState(PreviousState);
	//}
};