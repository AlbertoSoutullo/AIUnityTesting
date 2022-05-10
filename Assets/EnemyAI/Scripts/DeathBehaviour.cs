using UnityEngine;
using UnityEngine.AI;

namespace EnemyAI.New_Enemy.Scripts
{
    public class DeathBehaviour : StateMachineBehaviour
    {
        private static readonly int IsDead = Animator.StringToHash("isDead");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(IsDead, true);
            Destroy(animator.GetComponent<Rigidbody>());
            animator.GetComponent<CapsuleCollider>().enabled = false;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Destroy(animator.gameObject);
        }
    }
}