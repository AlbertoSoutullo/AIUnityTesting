// Unity Imports
using System.Linq;
using UnityEngine;

// Project Imports
using CustomFSM;

namespace HunterAI.Scripts.FSMStates
{
    public class FollowPlayerState : FsmState<Companion>
    {
        private float _lastTimeChecked;
        private CompanionMovement _movement;
        private CompanionLogic _logic;
        
        public static FollowPlayerState instance { get; } = new FollowPlayerState();

        static FollowPlayerState() {}

        public override void Enter(Companion companion) 
        {
            Debug.Log("Entering FollowPlayerState");
            _movement = companion.GetMovement();
            _logic = companion.GetLogic();
            _lastTimeChecked = Time.time;
        }

        public override void Execute(Companion companion)
        {
            if (InRangeToWalk(companion))
            {
                if (!NeedsToBeRedirected(companion)) return;
                MoveTowardsPlayer();
            }
            else if (_logic.EnemiesThatCanBeAttacked().Any())
            {
                Debug.Log("While following, Hunter can attack");
                companion.GetFsm().ChangeState(ChooseTargetState.instance);
            }
            else
            {
                Debug.Log("Close enough to player so changing to IdleState");
                companion.GetFsm().ChangeState(IdleState.instance);
            }
        }

        public override void Exit(Companion companion)
        {
            Debug.Log("Exit following");
            _movement.StopWalking();
        }

        private bool InRangeToWalk(Companion companion)
        {
            return (_logic.DistanceWithPlayer() >= companion.playerMaxDistanceToWalk);
        }
        
        private bool InRangeToRun()
        {
            return (_logic.DistanceWithPlayer() >= _logic.playerMaxDistanceToRun);
        }

        private bool NeedsToBeRedirected(Companion companion)
        {
            return (Time.time - _lastTimeChecked) > companion.movementRefreshTime;
        }

        private void MoveTowardsPlayer()
        {
            Debug.Log("Following player");
            Vector3 positionToWalk = _logic.GetRandomPositionWithinPlayerRange();
            _movement.WalkTo(positionToWalk, InRangeToRun());
            _lastTimeChecked = Time.time;
        }
    }
}