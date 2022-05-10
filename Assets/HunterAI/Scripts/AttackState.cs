// Unity Imports
using UnityEngine;

// Project Imports
using CustomFSM;

namespace HunterAI.Scripts
{
	public class AttackState : FsmState<CompanionMovement>
	{
		public static AttackState Instance { get; } = new AttackState();

		static AttackState() {}
		
		public override void Enter(CompanionMovement companion)
		{
			Debug.Log("Entering AttackState");
			
			companion.Attack();
		}

		public override void Execute(CompanionMovement companion)
		{
			if (companion.GetCurrentTarget() != null)
				companion.transform.LookAt(companion.GetCurrentTarget().transform.position);
		}

		public override void Exit(CompanionMovement entity) { }
	}
}