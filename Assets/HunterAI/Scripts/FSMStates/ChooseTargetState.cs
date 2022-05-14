// Unity Imports
using UnityEngine;

 // Project Imports
using CustomFSM;

namespace HunterAI.Scripts.FSMStates
{
	public class ChooseTargetState : FsmState<Companion>
	{
		private CompanionLogic _logic;
		public static ChooseTargetState instance { get; } = new ChooseTargetState();

		static ChooseTargetState() {}

		public override void Enter(Companion companion)
		{
			Debug.Log("Entering ChooseTargetState");
			_logic = companion.GetLogic();
			companion.SetCurrentTarget(_logic.GetClosestEnemy());
		}

		public override void Execute(Companion companion)
		{
			if (IsEnemyInRange(companion))
			{
				Debug.Log("Changing to AttackState");
				companion.GetFsm().ChangeState(AttackState.instance);
			}
			else
			{
				Debug.Log("Changing to RelocateState");
				companion.GetFsm().ChangeState(RelocateState.instance);
			}
		}

		public override void Exit(Companion companion) {}

		private bool IsEnemyInRange(Companion companion)
		{
			return Vector3.Distance(companion.GetCurrentTarget().transform.position, companion.transform.position)
			       < _logic.weaponRangeDistance;
		}
	}
}