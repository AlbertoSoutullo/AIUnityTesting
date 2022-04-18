using UnityEngine;

namespace HunterAI.Scripts
{
    public class FollowPlayerState : FsmState<CompanionMovement>
    {
        public static FollowPlayerState instance { get; } = new FollowPlayerState();

        static FollowPlayerState()
        {
        }

        public override void Enter(CompanionMovement companion)
        {
            Debug.Log("Entering FollowPlayerState");
        }

        public override void Execute(CompanionMovement companion)
        {
            // If further than companion.playerMaxDistance from the player,
            // just move towards the player
            Transform player = companion.GetPlayer();
            if (companion.DistanceWithPlayer() >= companion.playerMaxDistance)
                companion.WalkTo(player.position);
            // Whenever close enough to the player, go back to idle
            else
            {
                Debug.Log("Close enough to player so changing to IdleState");
                companion.GetFSM().ChangeState(IdleState.instance);
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