// Unity Imports
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
		
		IEnumerator OnCompleteAttackAnimation()
		{
			yield return new WaitUntil(() => _animationController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
			_animationController.SetBool(Shoot, false);
		}

		public bool IsAttackAnimationFinished()
		{
			return _animationController.GetCurrentAnimatorStateInfo(0).IsName("Shoot")
			       && _animationController.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
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
		
		public void FinishPointing()
		{
			_animationController.SetBool(NoAmmo, false);
			Debug.Log("Going back to IdleState");
			GetFsm().ChangeState(IdleState.instance);
		}

		public void FinishAttack()
		{
			_animationController.SetBool(Shoot, false);

			if (arrows <= 0)
			{
				Debug.Log("No arrows left so changing to PointState");
				GetFsm().ChangeState(PointState.instance);
			}
			else
			{
				GameObject currentClosestEnemy = GetClosestEnemy();
				if (currentClosestEnemy == null)
				{
					Debug.Log("No enemies can be attacked anymore so changing to IdleState");
					GetFsm().ChangeState(IdleState.instance);
				}
				else if (currentClosestEnemy != _currentTarget)
				{
					Debug.Log("Current target is no longer the closest so changing to ChooseTargetState");
					GetFsm().ChangeState(ChooseTargetState.instance);
				}
				else
				{
					if (Vector3.Distance(transform.position, _currentTarget.transform.position) > weaponRangeDistance)
					{
						Debug.Log("No longer in current target's range so changing to RelocateState");
						GetFsm().ChangeState(RelocateState.instance);
					}
					else
						GetFsm().ChangeState(AttackState.instance);
				}
			}
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
