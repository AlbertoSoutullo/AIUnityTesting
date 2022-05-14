// Unity Imports
using UnityEngine; 

// Project Imports
using CustomFSM;

namespace HunterAI.Scripts.FSMStates
{
	public class RelocateState : FsmState<Companion>
	{
		private CompanionMovement _movement;
		private CompanionLogic _logic;
		private bool _desiredLocationComputed;
		private Vector3 _desiredLocation;
		
		public static RelocateState instance { get; } = new RelocateState();
		
		static RelocateState() { }

		public override void Enter(Companion companion)
		{
			Debug.Log("Entering RelocateState");
			_desiredLocationComputed = false;
			_movement = companion.GetMovement();
			_logic = companion.GetLogic();
		}

		public override void Execute(Companion companion)
		{
			if (!_desiredLocationComputed)
			{
				ComputeLocation(companion);
			}
			else
				_movement.WalkTo(_desiredLocation, true);
			
			if (InDesiredLocation(companion))
			{
				Debug.Log("Arrived in desired location so changing to AttackState");
				_movement.StopWalking();
				companion.GetFsm().ChangeState(AttackState.instance);
			}
		}

		public override void Exit(Companion companion) { }

		private void ComputeLocation(Companion companion)
		{
			Debug.Log("Computing location to attack current target while within player's range");
			_desiredLocation = companion.GetPlayerTransform().position + 
			                   (companion.GetCurrentTarget().transform.position - companion.GetPlayerTransform().position) * 
			                   _logic.playerMaxDistanceToRun / (_logic.playerMaxDistanceToRun + _logic.weaponRangeDistance);
			_desiredLocationComputed = true;
		}

		private bool InDesiredLocation(Companion companion)
		{
			return (Vector3.Distance(_desiredLocation, companion.transform.position) < 1);
		}
	}
}