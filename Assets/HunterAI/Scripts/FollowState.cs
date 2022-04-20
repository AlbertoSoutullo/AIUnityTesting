using System.Linq;
using UnityEngine;

namespace HunterAI.Scripts
{
    public class FollowPlayerState : FsmState<CompanionMovement>
    {
        private float _lastTimeChecked;
        
        public static FollowPlayerState instance { get; } = new FollowPlayerState();

        static FollowPlayerState() {}

        public override void Enter(CompanionMovement companion) 
        {
            Debug.Log("Entering FollowPlayerState");
            _lastTimeChecked = Time.time;
        }

        public override void Execute(CompanionMovement companion)
        {
            if (companion.DistanceWithPlayer() >= companion.playerMaxDistance)
            {
                if (!NeedsToBeRedirected(companion)) return;
                
                Vector3 positionToWalk = companion.GetRandomPositionWithinPlayerRange();
                companion.WalkTo(positionToWalk);
                _lastTimeChecked = Time.time;
            }
            
            else if (companion.EnemiesThatCanBeAttacked().Any())
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

        public override void Exit(CompanionMovement companion) {}

        private bool NeedsToBeRedirected(CompanionMovement companion)
        {
            return (Time.time - _lastTimeChecked) > companion.movementRefreshTime;
        }
    }
}