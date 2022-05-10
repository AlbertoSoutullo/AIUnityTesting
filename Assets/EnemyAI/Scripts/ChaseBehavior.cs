using UnityEngine;
using UnityEngine.AI;

namespace EnemyAI.New_Enemy.Scripts
{
    public class ChaseBehavior : StateMachineBehaviour
    {
        public float chaseSpeed;
        public float attackRange;
        
        private NavMeshAgent _agent;
        private Transform _player;
        
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _agent = animator.GetComponent<NavMeshAgent>();
            _agent.speed = chaseSpeed;

            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var playerPosition = _player.position;
            
            _agent.SetDestination(playerPosition);
            float distanceToPlayer = Vector3.Distance(animator.transform.position, playerPosition);
            if (distanceToPlayer < attackRange)
                animator.SetBool(IsAttacking, true);
        }
    
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _agent.SetDestination(_agent.transform.position);
            _agent.speed = 3;
        }
    }
}
