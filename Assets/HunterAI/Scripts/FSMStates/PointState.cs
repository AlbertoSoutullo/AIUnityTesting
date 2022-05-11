// Unity Imports
using UnityEngine;

// Project Imports
using CustomFSM;

namespace HunterAI.Scripts.FSMStates
{
    public class PointState : FsmState<Companion>
    {
        private CompanionMovement _movement;
        private CompanionLogic _logic;
        
        public static PointState instance { get; } = new PointState();

        private GameObject _arrow;
        static PointState() {}

        public override void Enter(Companion companion)
        {
            Debug.Log("Entering PointState");
            _logic = companion.GetLogic();
            _arrow = _logic.GetClosestArrow();
            if (_arrow != null)
            {
                companion.transform.LookAt(_arrow.transform.position);
                companion.Point();
            }
        }

        public override void Execute(Companion companion)
        {
            Debug.Log("Executing in point state");
            if (_arrow != null)
            {
                companion.transform.LookAt(_arrow.transform.position);
            }

            if (companion.arrows > 0)
                companion.GetFsm().ChangeState(IdleState.instance);
        }

        public override void Exit(Companion companion) { }
    }
}