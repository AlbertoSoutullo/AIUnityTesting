using UnityEngine;

namespace EnemyAI.New_Enemy.Scripts
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