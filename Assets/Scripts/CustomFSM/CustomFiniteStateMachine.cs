namespace CustomFSM
{
	public class CustomFiniteStateMachine <T>  {
		
		private readonly T _owner;
		private FsmState<T> _currentState;

		public CustomFiniteStateMachine(T owner)
		{
			_owner = owner;
			_currentState = null;
		}

		public void Update()
		{
			_currentState?.Execute(_owner);
		}

		public void ChangeState(FsmState<T> newState)
		{
			_currentState?.Exit(_owner);
			_currentState = newState;
			_currentState?.Enter(_owner);
		}
	};
}