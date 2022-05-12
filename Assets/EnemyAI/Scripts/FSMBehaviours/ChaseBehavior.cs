// Unity Imports
using UnityEngine;
using UnityEngine.AI;

namespace EnemyAI.Scripts.FSMBehaviours
{
    public class ChaseBehavior : StateMachineBehaviour
    {
        public float normalSpeed = 2;
        public float chaseSpeed = 5;
        public float attackRange = 2;
        
        private NavMeshAgent _agent;
        private Transform _player;
        
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SetChaseSpeed(animator);
            FindPlayer();
        }
    
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Vector3 playerPosition = _player.position;
            ChasePlayer(playerPosition);
            AttackIfCloseEnough(animator, playerPosition);
        }
    
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _agent.SetDestination(_agent.transform.position);
            _agent.speed = normalSpeed;
        }

        private void SetChaseSpeed(Animator animator)
        {
            _agent = animator.GetComponent<NavMeshAgent>();
            _agent.speed = chaseSpeed;
        }
        
        private void FindPlayer()
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void ChasePlayer(Vector3 playerPosition)
        {
            _agent.SetDestination(playerPosition);
        }

        private void AttackIfCloseEnough(Animator animator, Vector3 playerPosition)
        {
            float distanceToPlayer = Vector3.Distance(animator.transform.position, playerPosition);
            if (distanceToPlayer < attackRange)
                animator.SetBool(IsAttacking, true);
        }
    }
}
