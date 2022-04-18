using UnityEngine;

namespace HunterAI.Scripts
{
    public class PointState : FsmState<CompanionMovement>
    {
        static readonly PointState instance = new PointState();
        public static PointState Instance { get { return instance; } }

        static PointState()
        {
        }
        PointState()
        {
        }

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
                companion.GetFSM().ChangeState(IdleState.instance);
        }

        public override void Execute(CompanionMovement companion)
        {

            //Debug.Log("Going back to IdleState");
            //companion.GetFSM().ChangeState(IdleState.Instance);
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