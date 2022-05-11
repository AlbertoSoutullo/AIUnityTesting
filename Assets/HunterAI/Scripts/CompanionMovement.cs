// Unity Imports
using UnityEngine;
using UnityEngine.AI;

namespace HunterAI.Scripts
{
    public class CompanionMovement: MonoBehaviour
    {
        private Animator _animationController;
        private Rigidbody _rigidbody;
        private NavMeshAgent _navMeshAgent;
        
        private static readonly int SpeedForAnimations = Animator.StringToHash("speed");
        private static readonly int IsFollowing = Animator.StringToHash("isFollowing");
        private static readonly int Running = Animator.StringToHash("running");

        void Start()
        {
            _animationController = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }
        
        public void StopWalking()
        {
            _navMeshAgent.SetDestination(transform.position);
            _rigidbody.velocity = Vector3.zero;
            _animationController.SetFloat(SpeedForAnimations, 0);
            _animationController.SetBool(IsFollowing, false);
            _animationController.SetBool(Running, false);
        }
        
        public void WalkTo(Vector3 destination, bool running)
        {
            _animationController.SetFloat(SpeedForAnimations, _rigidbody.velocity.sqrMagnitude);
            _animationController.SetBool(IsFollowing, true);
            if (running)
                _animationController.SetBool(Running, true);
            _navMeshAgent.SetDestination(destination);
            transform.LookAt(destination);
        }
    }
}