using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomFSM;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace HunterAI.Scripts
{
	public class CompanionMovement : MonoBehaviour
	{
		public float playerMaxDistanceToRun = 10f;
		public float playerMaxDistanceToWalk = 5f;
		public float weaponRangeDistance = 8;
		public int arrows = 10;
		public float visionRangeDistance = 25;
		public bool weaponIsCharged = true;
		public float movementRefreshTime = 2.0f;
		
		public float radius = 5.0f;
		
		private GameObject _currentTarget;
		private Transform _player;
		private Animator _animationController;
		private Rigidbody _rigidbody;
		private NavMeshAgent _navMeshAgent;
		private CustomFiniteStateMachine<CompanionMovement> _stateMachine;

		private static readonly int SpeedForAnimations = Animator.StringToHash("speed");
		
		public static event Action<CompanionMovement> InstanceStarted;
		
		public Transform GetPlayer()
		{
			return _player;
		}

		public GameObject GetCurrentTarget()
		{
			return _currentTarget;
		}

		public void SetCurrentTarget(GameObject target)
		{
			_currentTarget = target;
		}

		public CustomFiniteStateMachine<CompanionMovement> GetFsm()
		{
			return _stateMachine;
		}

		public IEnumerable<GameObject> SeeEnemies()
		{
			return GameObject.FindGameObjectsWithTag("Enemy").Where(enemy => 
				Vector3.Distance(transform.position, enemy.transform.position) < visionRangeDistance);
		}
		
		public IEnumerable<GameObject> EnemiesThatCanBeAttacked()
		{
			return GameObject.FindGameObjectsWithTag("Enemy")
				.Where(enemy => Vector3.Distance(transform.position, enemy.transform.position) < visionRangeDistance
				                && Vector3.Distance(_player.position, enemy.transform.position) < (weaponRangeDistance + playerMaxDistanceToRun));
		}

		public IEnumerable<GameObject> ExistingArrows()
		{
			return GameObject.FindGameObjectsWithTag("SpawnedArrow")
				.Where(arrow => Vector3.Distance(transform.position, arrow.transform.position) < visionRangeDistance);
		}
		
		public IEnumerable<GameObject> EnemiesInWeaponRange()
		{
			return GameObject.FindGameObjectsWithTag("Enemy")
				.Where(enemy => Vector3.Distance(transform.position, enemy.transform.position) < weaponRangeDistance);
		}
    
		public float DistanceWithPlayer()
		{
			return Vector3.Distance(transform.position, _player.position);
		}
		
		public GameObject GetClosestEnemy()
		{
			IEnumerable<GameObject> closeEnemies = EnemiesThatCanBeAttacked();
			GameObject targetEnemy = null;
			float minimumDistance = float.MaxValue;

			foreach (GameObject enemy in closeEnemies)
			{
				float distanceToPlayer = Vector3.Distance(enemy.transform.position, _player.position);
				if (distanceToPlayer < minimumDistance)
				{
					minimumDistance = distanceToPlayer;
					targetEnemy = enemy;
				}
			}
			return targetEnemy;
		}

		public GameObject GetClosestArrow()
		{
			IEnumerable<GameObject> arrows = ExistingArrows();
			GameObject targetArrow = null;
			float minimumDistance = float.MaxValue;

			foreach (GameObject arrow in arrows)
			{
				float distanceToPlayer = Vector3.Distance(arrow.transform.position, _player.position);
				if (distanceToPlayer < minimumDistance)
				{
					minimumDistance = distanceToPlayer;
					targetArrow = arrow;
				}
			}
			return targetArrow;
		}
		
		public void StopWalking()
		{
			_navMeshAgent.SetDestination(transform.position);
			_rigidbody.velocity = Vector3.zero;
			_animationController.SetFloat(SpeedForAnimations, 0);
			_animationController.SetBool("isFollowing", false);
			_animationController.SetBool("running", false);
		}
		
		public void WalkTo(Vector3 destination, bool running)
		{
			_animationController.SetFloat("speed", _rigidbody.velocity.sqrMagnitude);
			_animationController.SetBool("isFollowing", true);
			if (running)
				_animationController.SetBool("running", true);
			_navMeshAgent.SetDestination(destination);
			transform.LookAt(destination);
		}

		public Vector3 GetRandomPositionWithinPlayerRange()
		{
			Vector3 playerPositon = _player.position;
			float angle = Random.Range(0, 2f * Mathf.PI);
			
			return playerPositon + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
		}

		IEnumerator OnCompleteAttackAnimation()
		{
			yield return new WaitUntil(() => _animationController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
			_animationController.SetBool("shoot", false);
		}

		public bool IsAttackAnimationFinished()
		{
			return _animationController.GetCurrentAnimatorStateInfo(0).IsName("Shoot")
			       && _animationController.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
		}

		public void Attack()
		{
			_animationController.SetBool("shoot", true);
			arrows -= 1;
		}

		public void Point()
		{
			_animationController.SetBool("noAmmo", true);
		}
		
		public void FinishPointing()
		{
			_animationController.SetBool("noAmmo", false);
			Debug.Log("Going back to IdleState");
			GetFsm().ChangeState(IdleState.instance);
		}

		public void FinishAttack()
		{
			_animationController.SetBool("shoot", false);

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
					GetFsm().ChangeState(ChooseTargetState.Instance);
				}
				else
				{
					if (Vector3.Distance(transform.position, _currentTarget.transform.position) > weaponRangeDistance)
					{
						Debug.Log("No longer in current target's range so changing to RelocateState");
						GetFsm().ChangeState(RelocateState.Instance);
					}
					else
						GetFsm().ChangeState(AttackState.Instance);
					// else
					// {
					// Debug.Log("In current target's range so keeping in AttackState");
					// GetFSM().ChangeState(AttackState.Instance);
					// }
				}
			}
		}
		
		void Start()
		{
			_animationController = GetComponent<Animator>();
			_rigidbody = GetComponent<Rigidbody>();
			_navMeshAgent = GetComponent<NavMeshAgent>();
			_player = GameObject.FindGameObjectWithTag("Player").transform;

			_stateMachine = new CustomFiniteStateMachine<CompanionMovement>(this);
			GetFsm().ChangeState(IdleState.instance);
			InstanceStarted?.Invoke(this);
		}
		
		void Update ()
		{
			GetFsm().Update();
		}
	}
}
