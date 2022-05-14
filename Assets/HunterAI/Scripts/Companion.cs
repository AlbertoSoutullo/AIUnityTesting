// Unity Imports
using System;
using UnityEngine;

// Project Imports
using CustomFSM;
using HunterAI.Scripts.FSMStates;

namespace HunterAI.Scripts
{
	public class Companion : MonoBehaviour
	{
		public int arrows = 10;
		public float playerMaxDistanceToWalk = 5f;
		public float movementRefreshTime = 2.0f;
		
		private GameObject _currentTarget;
		private Transform _playerTransform;
		private Animator _animationController;
		
		private CustomFiniteStateMachine<Companion> _stateMachine;
		
		private static readonly int Shoot = Animator.StringToHash("shoot");
		private static readonly int NoAmmo = Animator.StringToHash("noAmmo");

		public static event Action<Companion> InstanceStarted;
		
		void Start()
		{
			_animationController = GetComponent<Animator>();
			_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

			_stateMachine = new CustomFiniteStateMachine<Companion>(this);
			GetFsm().ChangeState(IdleState.instance);
			InstanceStarted?.Invoke(this);
		}
		
		void Update ()
		{
			GetFsm().Update();
		}
		
		public Transform GetPlayerTransform()
		{
			return _playerTransform;
		}

		public GameObject GetCurrentTarget()
		{
			return _currentTarget;
		}

		public void SetCurrentTarget(GameObject target)
		{
			_currentTarget = target;
		}

		public CustomFiniteStateMachine<Companion> GetFsm()
		{
			return _stateMachine;
		}

		public void Attack()
		{
			_animationController.SetBool(Shoot, true);
			arrows -= 1;
		}

		public void Point()
		{
			_animationController.SetBool(NoAmmo, true);
		}
		
		public CompanionMovement GetMovement()
		{
			return gameObject.GetComponent<CompanionMovement>();
		}

		public CompanionLogic GetLogic()
		{
			return gameObject.GetComponent<CompanionLogic>();
		}
	}
}
