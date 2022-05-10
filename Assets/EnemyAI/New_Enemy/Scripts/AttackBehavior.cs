using UnityEngine;
using UnityEngine.AI;

namespace EnemyAI.New_Enemy.Scripts
{
    public class AttackBehavior : StateMachineBehaviour
    {
        private Transform _player;
        
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
        private static readonly int IsDead = Animator.StringToHash("isDead");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Enemy attacking");
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.GetBool(IsDead)) return;
            
            Transform transform = animator.transform;
            transform.LookAt(_player);
            animator.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

            if (distanceToPlayer > 3)
            {
                animator.SetBool(IsAttacking, false);
                //animator.GetComponent<NavMeshAgent>().isStopped = true;
            }
        }
    
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePosition;
        }
    }
}
