namespace HunterAI.Scripts
{
	public class CustomFiniteStateMachine <T>  {
		
		private readonly T _owner;
		private FsmState<T> _currentState;
		private FsmState<T> _previousState;
		private FsmState<T> _globalState;

		public FsmState<T> CurrentState { get { return _currentState; } set { _currentState = value; } }
		public FsmState<T> PreviousState { get { return _previousState; } set { _previousState = value; } }
		public FsmState<T> GlobalState { get { return _globalState; } set { _globalState = value; } }
		
		public CustomFiniteStateMachine(T owner)
		{
			this._owner = owner;
			this._currentState = null;
			this._previousState = null;
			this._globalState = null;
		}

		public void Update()
		{
			_globalState?.Execute(_owner);
			_currentState?.Execute(_owner);
		}

		public void ChangeState(FsmState<T> newState)
		{
			PreviousState = _currentState;
			
			_currentState?.Exit(_owner);
			_currentState = newState;
			_currentState?.Enter(_owner);
		}
	};
}