using UnityEngine;

namespace HunterAI.Scripts
{
    public class RechargeState : FsmState<CompanionMovement>
    {
        static readonly RechargeState instance = new RechargeState();
        public static RechargeState Instance { get { return instance; } }

        static RechargeState()
        {
        }
        RechargeState()
        {
        }

        public override void Enter(CompanionMovement companion)
        {
            Debug.Log("Entering RechargeState");
        }

        public override void Execute(CompanionMovement companion)
        {
            // If the companion's weapon is not charged,
            // charge it then attack
            if (!companion.weaponIsCharged)
            {
                Debug.Log("Charging weapon");
                companion.weaponIsCharged = true;
            }
            Debug.Log("Weapon is charged so changing to AttackState");
            companion.GetFSM().ChangeState(AttackState.Instance);

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