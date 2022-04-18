using UnityEngine;

namespace EnemyAI.New_Enemy.Scripts
{
    public class IdleBehavior : StateMachineBehaviour
    {
        private float _timer;
        private Transform _player;
        private const int StartPatrolling = 5;
        private const float ChaseRange = 15;
        
        private static readonly int IsPatrolling = Animator.StringToHash("isPatrolling");
        private static readonly int IsChasing = Animator.StringToHash("isChasing");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _timer = 0;
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _timer += Time.deltaTime;
            if (_timer > StartPatrolling)
                animator.SetBool(IsPatrolling, true);

            float distance = Vector3.Distance(animator.transform.position, _player.position);
            if (distance < ChaseRange)
                animator.SetBool(IsChasing, true);
        }
    
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        
        }
    }
}
