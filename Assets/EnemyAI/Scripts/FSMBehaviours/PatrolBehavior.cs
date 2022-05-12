// Unity Imports
using UnityEngine;
using UnityEngine.AI;

namespace EnemyAI.Scripts.FSMBehaviours
{
    public class PatrolBehavior : StateMachineBehaviour
    {
        private float _timer; 
        private NavMeshAgent _agent;
        private Transform _player;
        
        private const float ChaseRange = 15;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // todo go to random position and start timer
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // todo move after a given period of time
            // todo check if player is nearby
        }
        
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }
    }
}
