using UnityEngine;

namespace HunterAI.Scripts
{
public class RelocateState : FsmState<CompanionMovement>
	{
		static readonly RelocateState instance = new RelocateState();
		public static RelocateState Instance { get { return instance; } }

		private bool desiredLocationSet;
		private Vector3 desiredLocation;

		static RelocateState()
		{
		}
		RelocateState()
		{
		}

		public override void Enter(CompanionMovement companion)
		{
			Debug.Log("Entering RelocateState");
			desiredLocationSet = false;
		}

		public override void Execute(CompanionMovement companion)
		{
			// First of all compute the location to go
			// (close enough to both player and target)
			if (!desiredLocationSet)
			{
				Debug.Log("Computing location to attack current target while within player's range");
				desiredLocation =
					companion.GetPlayer().transform.position
					+ (companion.currentTarget.transform.position - companion.GetPlayer().transform.position)
					* companion.playerMaxDistance / (companion.playerMaxDistance + companion.weaponRangeDistance);
				desiredLocationSet = true;
			}
			// Then move to said location and attack
			else
				companion.WalkTo(desiredLocation);
			if (Vector3.Distance(desiredLocation, companion.transform.position) < 1)
			{
				Debug.Log("Arrived in desired location so changing to AttackState");
				companion.StopWalking();
				companion.GetFSM().ChangeState(AttackState.Instance);
			}


		}

		public override void Exit(CompanionMovement companion)
		{
		}

		// Start is called before the first frame update
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}