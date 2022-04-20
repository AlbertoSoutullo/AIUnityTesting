using UnityEngine;

namespace HunterAI.Scripts
{
    public class PointState : FsmState<CompanionMovement>
    {
        public static PointState instance { get; } = new PointState();

        static PointState() {}

        public override void Enter(CompanionMovement companion)
        {
            Debug.Log("Entering PointState");
            GameObject arrow = companion.GetClosestArrow();
            if (arrow != null)
            {
                companion.transform.LookAt(arrow.transform.position);
                companion.Point();
            }
            else
                companion.GetFsm().ChangeState(IdleState.instance);
        }

        public override void Execute(CompanionMovement companion)
        {
            Debug.Log("Executing in point state");
            //companion.GetFSM().ChangeState(IdleState.Instance);
        }

        public override void Exit(CompanionMovement companion)
        {
            Debug.Log("EXITING POINT STATE");
        }
    }
}