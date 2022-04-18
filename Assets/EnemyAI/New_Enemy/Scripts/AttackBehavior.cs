using UnityEngine;

namespace EnemyAI.New_Enemy.Scripts
{
    public class AttackBehavior : StateMachineBehaviour
    {
        private Transform _player;
        
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Transform transform = animator.transform;
            transform.LookAt(_player);
            float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
            
            if (distanceToPlayer > 3)
                animator.SetBool(IsAttacking, false);
        }
    
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        
        }
    }
}
