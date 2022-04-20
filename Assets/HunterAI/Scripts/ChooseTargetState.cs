using UnityEngine;

namespace HunterAI.Scripts
{
public class ChooseTargetState : FsmState<CompanionMovement>
	{
		public static ChooseTargetState instance { get; } = new ChooseTargetState();

		static ChooseTargetState() {}

		public override void Enter(CompanionMovement companion)
		{
			Debug.Log("Entering ChooseTargetState");
		}

		public override void Execute(CompanionMovement companion)
		{
			companion.SetCurrentTarget(companion.GetClosestEnemy());
			
			if (Vector3.Distance(companion.GetCurrentTarget().transform.position, companion.transform.position) 
			    < companion.weaponRangeDistance)
			{
				Debug.Log("An enemy is close enough so changing to AttackState");
				companion.GetFsm().ChangeState(AttackState.instance);
			}
			else
			{
				Debug.Log("Too far from enemy so changing to RelocateState");
				companion.GetFsm().ChangeState(RelocateState.Instance);
			}
		}

		public override void Exit(CompanionMovement companion) {}
	}
}