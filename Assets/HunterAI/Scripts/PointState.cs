using UnityEngine;

namespace HunterAI.Scripts
{
    public class PointState : FsmState<CompanionMovement>
    {
        public static PointState instance { get; } = new PointState();

        static PointState() {}

        public override void Enter(CompanionMovement companion)
        {
            // Debug.Log("Entering PointState");
            GameObject arrow = companion.GetClosestArrow();
            if (arrow != null)
            {
                companion.transform.LookAt(arrow.transform.position);
                companion.Point();
            }
        }

        public override void Execute(CompanionMovement companion)
        {
            Debug.Log("Executing in point state");

            if (companion.arrows > 0)
                companion.GetFsm().ChangeState(IdleState.instance);
        }

        public override void Exit(CompanionMovement companion)
        {
            Debug.Log("EXITING POINT STATE");
        }
    }
}