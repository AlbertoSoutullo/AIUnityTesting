// Unity Imports
using System.Linq;
using UnityEngine;

// Project Imports
using CustomFSM;

namespace HunterAI.Scripts.FSMStates
{
    public class IdleState : FsmState<Companion>
    {
        private CompanionMovement _movement;
        private CompanionLogic _logic;
        
        public static IdleState instance { get; } = new IdleState();
        
        static IdleState() {}

        public override void Enter(Companion companion)
        {
            Debug.Log("Entering IdleState");
            _movement = companion.GetMovement();
            _logic = companion.GetLogic();
        }

        public override void Execute(Companion companion)
        {
            _movement.StopWalking();

            if (_logic.DistanceWithPlayer() >= _logic.playerMaxDistanceToRun)
            {
                Debug.Log("Too far from player so changing to FollowPlayerState");
                companion.GetFsm().ChangeState(FollowPlayerState.instance);
            }
            else if (companion.arrows < 1)
            {
                Debug.Log("No arrows.");
                companion.GetFsm().ChangeState(PointState.instance);
            }
            else if (_logic.EnemiesThatCanBeAttacked().Any())
            {
                Debug.Log("An enemy can be attacked so changing to FollowPlayerState");
                companion.GetFsm().ChangeState(ChooseTargetState.instance);
            }
        }

        public override void Exit(Companion companion) {}
    }
}