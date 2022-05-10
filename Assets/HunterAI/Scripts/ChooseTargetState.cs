// Unity Imports
using UnityEngine;

// Project Imports
using CustomFSM;

namespace HunterAI.Scripts
{
	public class ChooseTargetState : FsmState<CompanionMovement>
	{
		public static ChooseTargetState Instance { get; } = new ChooseTargetState();

		static ChooseTargetState() {}

		public override void Enter(CompanionMovement companion)
		{
			Debug.Log("Entering ChooseTargetState");
			companion.SetCurrentTarget(companion.GetClosestEnemy());
		}

		public override void Execute(CompanionMovement companion)
		{
			if (IsEnemyInRange(companion))
			{
				Debug.Log("Changing to AttackState");
				companion.GetFsm().ChangeState(AttackState.Instance);
			}
			else
			{
				Debug.Log("Changing to RelocateState");
				companion.GetFsm().ChangeState(RelocateState.Instance);
			}
		}

		public override void Exit(CompanionMovement companion) {}

		private bool IsEnemyInRange(CompanionMovement companion)
		{
			return Vector3.Distance(companion.GetCurrentTarget().transform.position, companion.transform.position)
			       < companion.weaponRangeDistance;
		}
	}
}