// Unity Imports
using UnityEngine;

namespace EnemyAI.Scripts.FSMBehaviours
{
    public class AttackBehavior : StateMachineBehaviour
    {
        private Transform _player;
        
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
        private static readonly int IsDead = Animator.StringToHash("isDead");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Enemy attacking");
            FindPlayer();
        }
    
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.GetBool(IsDead)) return;
            
            LockPositionToAttack(animator);
            
            Transform transform = animator.transform;
            transform.LookAt(_player);
            
            AttackToPlayer(animator, transform);
        }
    
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.GetBool(IsDead)) return;
            animator.GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePosition;
        }

        private void FindPlayer()
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void LockPositionToAttack(Animator animator)
        {
            animator.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        }

        private void AttackToPlayer(Animator animator, Transform transform)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
            if (distanceToPlayer > 3)
                animator.SetBool(IsAttacking, false);
        }
    }
}
