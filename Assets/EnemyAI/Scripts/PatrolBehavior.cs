using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace EnemyAI.Scripts
{
    public class PatrolBehavior : StateMachineBehaviour
    {
        // private float _timer; todo
        private NavMeshAgent _agent;
        private Transform _player;
        
        private readonly List<Transform> _wayPoints = new List<Transform>();
        private const float ChaseRange = 35;

        private static readonly int IsPatrolling = Animator.StringToHash("isPatrolling");
        private static readonly int IsChasing = Animator.StringToHash("isChasing");
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // _timer = 0; todo
            //Transform wayPointsObject = GameObject.FindGameObjectWithTag("WayPoints").transform;
            //foreach (Transform t in wayPointsObject)
            //    _wayPoints.Add(t);

            _agent = animator.GetComponent<NavMeshAgent>();
            //_agent.SetDestination(_wayPoints[0].position);

            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                // _agent.SetDestination(_wayPoints[Random.Range(0, _wayPoints.Count)].position);
                animator.SetBool(IsPatrolling, false); 
            }

            //_timer += Time.deltaTime; todo
            //if (_timer > 10)
            //    animator.SetBool(IsPatrolling, false);

            float distanceToPlayer = Vector3.Distance(animator.transform.position, _player.position);
            if (distanceToPlayer < ChaseRange)
                animator.SetBool(IsChasing, true);
        }
        
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // _agent.SetDestination(_agent.transform.position); todo
        }
    }
}
