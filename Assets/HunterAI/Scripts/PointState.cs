// Unity Imports
using UnityEngine;

// Project Imports
using CustomFSM;

namespace HunterAI.Scripts
{
    public class PointState : FsmState<CompanionMovement>
    {
        public static PointState instance { get; } = new PointState();

        private GameObject _arrow;
        static PointState() {}

        public override void Enter(CompanionMovement companion)
        {
            // Debug.Log("Entering PointState");
            _arrow = companion.GetClosestArrow();
            if (_arrow != null)
            {
                companion.transform.LookAt(_arrow.transform.position);
                companion.Point();
            }
        }

        public override void Execute(CompanionMovement companion)
        {
            Debug.Log("Executing in point state");
            if (_arrow != null)
            {
                companion.transform.LookAt(_arrow.transform.position);
            }

            if (companion.arrows > 0)
                companion.GetFsm().ChangeState(IdleState.instance);
        }

        public override void Exit(CompanionMovement companion)
        {
            Debug.Log("EXITING POINT STATE");
        }
    }
}