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
            // First of all stop any possible ongoing walking animation
            companion.StopWalking();
            Transform player = companion.GetPlayer();
            // If too far from player, follow the player
            if (companion.DistanceWithPlayer() >= companion.playerMaxDistance)
            {
                Debug.Log("Too far from player so changing to FollowPlayerState");
                companion.GetFSM().ChangeState(FollowPlayerState.instance);
            }
            // If companion does not have any arrows, point to closest
            else if (companion.arrows < 1)
            {
                Debug.Log("No arrows.");
                companion.GetFSM().ChangeState(PointState.Instance);
            }
            // If any enemy can be attacked, go to attacking mode
            else if (companion.EnemiesThatCanBeAttacked().Count() > 0)
            {
                Debug.Log("An enemy can be attacked so changing to FollowPlayerState");
                companion.GetFSM().ChangeState(ChooseTargetState.Instance);
            }

        }

        public override void Exit(CompanionMovement companion)
        {
        }
    }
}