// Unity Imports
using UnityEngine; 

// Project Imports
using CustomFSM;

namespace HunterAI.Scripts.FSMStates
{
	public class AttackState : FsmState<Companion>
	{
		public static AttackState instance { get; } = new AttackState();

		static AttackState() {}
		
		public override void Enter(Companion companion)
		{
			Debug.Log("Entering AttackState");
			companion.Attack();
		}

		public override void Execute(Companion companion)
		{
			if (companion.GetCurrentTarget() != null)
				companion.transform.LookAt(companion.GetCurrentTarget().transform.position);
		}

		public override void Exit(Companion entity) { }
	}
}