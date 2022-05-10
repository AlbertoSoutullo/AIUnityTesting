// Unity Imports
using UnityEngine;

// Project Imports
using CustomFSM;

namespace HunterAI.Scripts
{
    public class RechargeState : FsmState<CompanionMovement>
    {
        static readonly RechargeState instance = new RechargeState();
        public static RechargeState Instance { get { return instance; } }

        static RechargeState() {}

        public override void Enter(CompanionMovement companion)
        {
            Debug.Log("Entering RechargeState");
        }

        public override void Execute(CompanionMovement companion)
        {
            if (!companion.weaponIsCharged)
            {
                Debug.Log("Charging weapon");
                companion.weaponIsCharged = true;
            }
            Debug.Log("Weapon is charged so changing to AttackState");
            companion.GetFsm().ChangeState(AttackState.Instance);
        }

        public override void Exit(CompanionMovement companion) {}
    }
}