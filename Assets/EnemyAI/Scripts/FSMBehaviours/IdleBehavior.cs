// Unity Imports
using UnityEngine;

namespace EnemyAI.Scripts.FSMBehaviours
{
    public class IdleBehavior : StateMachineBehaviour
    {
        private Transform _player;
        private const float ChaseRange = 15;
        private float _timer;
        private const float StartPatrollingIn = 4f;

        private static readonly int IsChasing = Animator.StringToHash("isChasing");
        private static readonly int IsPatrolling = Animator.StringToHash("isPatrolling");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _timer = Time.time;
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            animator.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        }
    
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ChasePlayerIfPossible(animator);
            GoToPatrol(animator);
        }

        private void ChasePlayerIfPossible(Animator animator)
        {
            float distance = Vector3.Distance(animator.transform.position, _player.position);
            if (distance < ChaseRange)
            {
                animator.GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePosition;
                animator.SetBool(IsChasing, true);
            }
        }

        private void GoToPatrol(Animator animator)
        {
            if ((Time.time - _timer) < StartPatrollingIn)
                animator.SetBool(IsPatrolling, true);
        }
    }
}
