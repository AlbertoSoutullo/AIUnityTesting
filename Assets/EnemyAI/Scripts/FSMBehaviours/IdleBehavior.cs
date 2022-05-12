// Unity Imports
using UnityEngine;

namespace EnemyAI.Scripts.FSMBehaviours
{
    public class IdleBehavior : StateMachineBehaviour
    {
        private Transform _player;
        private const float ChaseRange = 15;
        
        private static readonly int IsChasing = Animator.StringToHash("isChasing");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ChasePlayerIfPossible(animator);
        }

        private void ChasePlayerIfPossible(Animator animator)
        {
            float distance = Vector3.Distance(animator.transform.position, _player.position);
            if (distance < ChaseRange)
                animator.SetBool(IsChasing, true);
        }
    }
}
