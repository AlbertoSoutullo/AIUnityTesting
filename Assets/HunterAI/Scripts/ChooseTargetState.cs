using UnityEngine;

namespace HunterAI.Scripts
{
public class ChooseTargetState : FsmState<CompanionMovement>
	{
		static readonly ChooseTargetState instance = new ChooseTargetState();
		public static ChooseTargetState Instance { get { return instance; } }

		static ChooseTargetState()
		{
		}
		ChooseTargetState()
		{
		}

		public override void Enter(CompanionMovement companion)
		{
			Debug.Log("Entering ChooseTargetState");
		}

		public override void Execute(CompanionMovement companion)
		{
			// From those enemies that the companion can see,
			// select the closest to the player
			companion.currentTarget = companion.GetClosestEnemy();

			// If the companion is already in weapon range
			// with target, load the bow
			if (Vector3.Distance(companion.currentTarget.transform.position, companion.transform.position) < companion.weaponRangeDistance)
			{
				Debug.Log("An enemy is close enough so changing to AttackState");
				companion.GetFSM().ChangeState(AttackState.Instance);
			}
			// Otherwise, move close enough to target and then
			// load the bow
			else
			{
				Debug.Log("Too far from enemy so changing to RelocateState");
				companion.GetFSM().ChangeState(RelocateState.Instance);
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