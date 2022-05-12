// Unity Imports
using UnityEngine;
using UnityEngine.AI;

namespace EnemyAI.Scripts.FSMBehaviours
{
    public class PatrolBehavior : StateMachineBehaviour
    {
        private NavMeshAgent _agent;
        private Transform _player;

        private const float ChaseRange = 5;
        private bool _patrolling = false;
        private static readonly int IsChasing = Animator.StringToHash("IsChasing");
        private const float Radius = 10f;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _agent = animator.GetComponent<NavMeshAgent>();
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            animator.GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePosition;
            
            RandomWalk(animator);
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            CheckIfPlayerIsInChaseRange(animator);
            NewWalkPoint(animator);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }

        private void RandomWalk(Animator animator)
        {
            float angle = Random.Range(0, 2f * Mathf.PI);
			
            Vector3 positionToWalk = animator.transform.position + 
                                     new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * Radius;

            _agent.SetDestination(positionToWalk);
            _patrolling = true;
        }

        private void NewWalkPoint(Animator animator)
        {
            if (IsWalkCompleted())
                _patrolling = false;
            if (_patrolling) return;
            
            Debug.Log("New walk point");
            RandomWalk(animator);
        }

        private void CheckIfPlayerIsInChaseRange(Animator animator)
        {
            float distance = Vector3.Distance(animator.transform.position, _player.position);
            if (distance < ChaseRange)
            {
                animator.SetBool(IsChasing, true);
            }
        }

        private bool IsWalkCompleted()
        {
            if (_agent.pathPending) return false;
            if (!(_agent.remainingDistance <= _agent.stoppingDistance)) return false;
            
            return !_agent.hasPath || _agent.velocity.sqrMagnitude == 0f;
        }
    }
}
