using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HunterAI.Scripts
{
	

	

	

	

	

	

	public class AttackState : FsmState<CompanionMovement>
	{
		static readonly AttackState instance = new AttackState();
		public static AttackState Instance { get { return instance; } }

		private bool desiredLocationSet;
		private Vector3 desiredLocation;

		static AttackState()
		{
		}
		AttackState()
		{
		}
		public override void Enter(CompanionMovement companion)
		{
			Debug.Log("Entering AttackState");
			// First of all attack the target
			companion.Attack();
		}

		public override void Execute(CompanionMovement companion)
		{
			if (companion.currentTarget != null)
				companion.transform.LookAt(companion.currentTarget.transform.position);
			//if (companion.IsAttackAnimationFinished())
			//{
			//	companion.FinishAttack();

			//	// If no enemy can be attacked anymore, go back to idle
			//	GameObject currentClosestEnemy = companion.GetClosestEnemy();
			//	if (currentClosestEnemy == null)
			//	{
			//		Debug.Log("No enemies can be attacked anymore so changing to IdleState");
			//		companion.GetFSM().ChangeState(IdleState.Instance);
			//	}
			//	// If another enemy is closer, change target
			//	else if (currentClosestEnemy != companion.currentTarget)
			//	{
			//		Debug.Log("Current target is no longer the closest so changing to ChooseTargetState");
			//		companion.GetFSM().ChangeState(ChooseTargetState.Instance);

			//	}
			//	else
			//	{
			//		// If not close enough (I think it's impossible though)
			//		// relocate again
			//		if (Vector3.Distance(companion.transform.position, companion.currentTarget.transform.position) > companion.weaponRangeDistance)
			//		{
			//			Debug.Log("No longer in current target's range so changing to RelocateState");
			//			companion.GetFSM().ChangeState(RelocateState.Instance);
			//		}
			//		else
			//			companion.GetFSM().ChangeState(AttackState.Instance);
			//		// Otherwise recharge and attack again
			//		// else
			//		// {
			//		// Debug.Log("In current target's range so keeping in AttackState");
			//		// this.GetFSM().ChangeState(AttackState.Instance);
			//		// }
			//	}
			//}
			//else if (companion.currentTarget != null)
			//	companion.transform.LookAt(companion.currentTarget.transform.position);
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