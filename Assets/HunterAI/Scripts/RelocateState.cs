using UnityEngine;

namespace HunterAI.Scripts
{
public class RelocateState : FsmState<CompanionMovement>
	{
		static readonly RelocateState instance = new RelocateState();
		public static RelocateState Instance { get { return instance; } }

		private bool desiredLocationSet;
		private Vector3 desiredLocation;

		static RelocateState() {}

		public override void Enter(CompanionMovement companion)
		{
			Debug.Log("Entering RelocateState");
			desiredLocationSet = false;
		}

		public override void Execute(CompanionMovement companion)
		{
			if (!desiredLocationSet)
			{
				Debug.Log("Computing location to attack current target while within player's range");
				desiredLocation =
					companion.GetPlayer().position
					+ (companion.GetCurrentTarget().transform.position - companion.GetPlayer().position)
					* companion.playerMaxDistance / (companion.playerMaxDistance + companion.weaponRangeDistance);
				desiredLocationSet = true;
			}

			else
				companion.WalkTo(desiredLocation);
			if (Vector3.Distance(desiredLocation, companion.transform.position) < 1)
			{
				Debug.Log("Arrived in desired location so changing to AttackState");
				companion.StopWalking();
				companion.GetFsm().ChangeState(AttackState.instance);
			}
		}

		public override void Exit(CompanionMovement companion) {}
	}
}