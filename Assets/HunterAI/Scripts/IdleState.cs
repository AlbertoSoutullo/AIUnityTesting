using UnityEngine;
using System.Linq;

namespace HunterAI.Scripts
{
    public class IdleState : FsmState<CompanionMovement>
    {
        public static IdleState instance { get; } = new IdleState();
        
        static IdleState() {}

        public override void Enter(CompanionMovement companion)
        {
            Debug.Log("Entering IdleState");
        }

        public override void Execute(CompanionMovement companion)
        {
            companion.StopWalking();

            if (companion.DistanceWithPlayer() >= companion.playerMaxDistanceToRun)
            {
                Debug.Log("Too far from player so changing to FollowPlayerState");
                companion.GetFsm().ChangeState(FollowPlayerState.instance);
            }
            
            else if (companion.arrows < 1)
            {
                Debug.Log("No arrows.");
                companion.GetFsm().ChangeState(PointState.instance);
            }

            else if (companion.EnemiesThatCanBeAttacked().Any())
            {
                Debug.Log("An enemy can be attacked so changing to FollowPlayerState");
                companion.GetFsm().ChangeState(ChooseTargetState.instance);
            }
        }

        public override void Exit(CompanionMovement companion) {}
    }
}