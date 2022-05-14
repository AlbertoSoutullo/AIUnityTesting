//Unity Imports
using UnityEngine;

// Project Imports
using HunterAI.Scripts.FSMStates;

namespace HunterAI.Scripts
{
    public class CompanionAnimationEvents: MonoBehaviour
    {
        private Animator _animationController;

        private Companion _companion;
        private CompanionLogic _logic;
        
        private static readonly int Shoot = Animator.StringToHash("shoot");
        private static readonly int NoAmmo = Animator.StringToHash("noAmmo");
        
        private void Start()
        {
            _animationController = GetComponent<Animator>();
            _companion = GetComponent<Companion>();
            _logic = GetComponent<CompanionLogic>();
        }

        public void FinishPointing()
        {
            _animationController.SetBool(NoAmmo, false);
            Debug.Log("Going back to IdleState");
            _companion.GetFsm().ChangeState(IdleState.instance);
        }
        
        public void FinishAttack()
        {
            _animationController.SetBool(Shoot, false);

            if (_companion.arrows <= 0)
            {
                Debug.Log("No arrows left so changing to PointState");
                _companion.GetFsm().ChangeState(PointState.instance);
            }
            else
            {
                GameObject currentClosestEnemy = _logic.GetClosestEnemy();
                if (currentClosestEnemy == null)
                {
                    Debug.Log("No enemies can be attacked anymore so changing to IdleState");
                    _companion.GetFsm().ChangeState(IdleState.instance);
                }
                else if (currentClosestEnemy != _companion.GetCurrentTarget())
                {
                    Debug.Log("Current target is no longer the closest so changing to ChooseTargetState");
                    _companion.GetFsm().ChangeState(ChooseTargetState.instance);
                }
                else
                {
                    if (Vector3.Distance(transform.position, _companion.GetCurrentTarget().transform.position) > 
                        _logic.weaponRangeDistance)
                    {
                        Debug.Log("No longer in current target's range so changing to RelocateState");
                        _companion.GetFsm().ChangeState(RelocateState.instance);
                    }
                    else
                        _companion.GetFsm().ChangeState(AttackState.instance);
                }
            }
        }
    }
}