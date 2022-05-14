// Unity Imports
using UnityEngine;

namespace EnemyAI.Scripts.FSMBehaviours
{
    public class DamagedBehaviour: StateMachineBehaviour
    {
        private static readonly int Damaged = Animator.StringToHash("Damaged");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetTrigger(Damaged);
        }
    }
}