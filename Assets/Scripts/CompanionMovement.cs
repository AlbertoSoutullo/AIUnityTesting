
using System;
using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CompanionMovement : MonoBehaviour
{
    public Transform player;
    
    private static readonly int speedForAnimations = Animator.StringToHash("speed");

    private Animator _animationController;
    private Rigidbody _rigidbody;
    public UnityEngine.AI.NavMeshAgent _navMeshAgent;
    
    FiniteStateMachine<CompanionMovement> stateMachine;

    public float playerMaxDistance = 5;
    public float weaponRangeDistance = 8;
    public int arrows = 10;
    public float visionRangeDistance = 25;
    public bool weaponIsCharged = true;
    public GameObject currentTarget = null;
    public float Speed;

    public FiniteStateMachine<CompanionMovement> GetFSM()
    {
      return stateMachine;
    }

    public IEnumerable<GameObject> SeeEnemies()
    {
      return GameObject.FindGameObjectsWithTag("Enemy").Where(enemy => Vector3.Distance(transform.position, enemy.transform.position) < visionRangeDistance);
    }

    // An enemy can be attacked if the companion can locate itself than weaponRangeDistance
    // while maintaining itself closer than playerMaxDistance to the player
    public IEnumerable<GameObject> EnemiesThatCanBeAttacked()
    {
      return GameObject.FindGameObjectsWithTag("Enemy")
                     .Where(enemy => Vector3.Distance(transform.position, enemy.transform.position) < visionRangeDistance
                               && Vector3.Distance(player.position, enemy.transform.position) < (weaponRangeDistance + playerMaxDistance));
    }

    public IEnumerable<GameObject> ExistingArrows()
    {
        return GameObject.FindGameObjectsWithTag("SpawnedArrow")
            .Where(arrow => Vector3.Distance(transform.position, arrow.transform.position) < visionRangeDistance);
    }
    
    // Enemies that are closer than weaponRangeDistance to the companion
    public IEnumerable<GameObject> EnemiesInWeaponRange()
    {
      return GameObject.FindGameObjectsWithTag("Enemy")
               .Where(enemy => Vector3.Distance(transform.position, enemy.transform.position) < weaponRangeDistance);
    }
    
    public float DistanceWithPlayer()
    {
        return Vector3.Distance(this.transform.position, this.player.position);
    }

    // Returns the closest enemy that can be attacked by the companion
    public GameObject GetClosestEnemy()
    {
        // Iterate over all those enemies that can be attacked
        // (close enough to player so that the companion can attack them
        // while not being to far from the player)
        IEnumerable<GameObject> closeEnemies = EnemiesThatCanBeAttacked();
        GameObject targetEnemy = null;
        float minimumDistance = float.MaxValue;
        // Find the closest enemy to player that can be attacked
        foreach (GameObject enemy in closeEnemies)
        {
            float distanceToPlayer = Vector3.Distance(enemy.transform.position, this.player.position);
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
            float distanceToPlayer = Vector3.Distance(arrow.transform.position, this.player.position);
            if (distanceToPlayer < minimumDistance)
            {
                minimumDistance = distanceToPlayer;
                targetArrow = arrow;
            }
        }
        return targetArrow;
    }
    
    // Simple function to stop any walking animation (NavMeshAgent too)
    public void StopWalking()
    {
        this._navMeshAgent.SetDestination(this.transform.position);
        this._animationController.SetFloat(speedForAnimations, 0);
    }

    // Simple function to walk to a certain destination using the NavMeshAgent
    public void WalkTo(Vector3 destination)
    {
        this._navMeshAgent.SetDestination(destination);
        this._animationController.SetFloat("speed", this._rigidbody.velocity.magnitude);
        this.transform.LookAt(destination);
    }

    IEnumerator OnCompleteAttackAnimation()
    {
        yield return new WaitUntil(() => this._animationController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
        this._animationController.SetBool("shoot", false);
    }

    public bool IsAttackAnimationFinished()
    {
        return this._animationController.GetCurrentAnimatorStateInfo(0).IsName("Shoot")
               && this._animationController.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
    }

    public void Attack()
    {
        this._animationController.SetBool("shoot", true);
        arrows -= 1;
    }

    public void Point()
    {
        this._animationController.SetBool("noAmmo", true);
    }

    public void FinishPointing()
    {
        this._animationController.SetBool("noAmmo", false);
		Debug.Log("Going back to IdleState");
		this.GetFSM().ChangeState(IdleState.Instance);
	}

	public void FinishAttack()
    {
		this._animationController.SetBool("shoot", false);

		if (this.arrows <= 0)
		{
			Debug.Log("No arrows left so changing to PointState");
			this.GetFSM().ChangeState(PointState.Instance);
		}
		else
		{
			// If no enemy can be attacked anymore, go back to idle
			GameObject currentClosestEnemy = this.GetClosestEnemy();
			if (currentClosestEnemy == null)
			{
				Debug.Log("No enemies can be attacked anymore so changing to IdleState");
				this.GetFSM().ChangeState(IdleState.Instance);
			}
			// If another enemy is closer, change target
			else if (currentClosestEnemy != this.currentTarget)
			{
				Debug.Log("Current target is no longer the closest so changing to ChooseTargetState");
				this.GetFSM().ChangeState(ChooseTargetState.Instance);

			}
			else
			{
				// If not close enough (I think it's impossible though)
				// relocate again
				if (Vector3.Distance(this.transform.position, this.currentTarget.transform.position) > this.weaponRangeDistance)
				{
					Debug.Log("No longer in current target's range so changing to RelocateState");
					this.GetFSM().ChangeState(RelocateState.Instance);
				}
				else
					this.GetFSM().ChangeState(AttackState.Instance);
				// Otherwise recharge and attack again
				// else
				// {
				// Debug.Log("In current target's range so keeping in AttackState");
				// this.GetFSM().ChangeState(AttackState.Instance);
				// }
			}
		}
	}

	// Start is called before the first frame update
	void Start()
    {
        this._animationController = GetComponent<Animator>();
        this._rigidbody = GetComponent<Rigidbody>();
        this._navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		this.player = GameObject.FindGameObjectsWithTag("Player")[0].transform;

		this.stateMachine = new FiniteStateMachine<CompanionMovement>(this);
        GetFSM().ChangeState(IdleState.Instance);
    }

    // Update is called once per frame
    void Update ()
    {
        GetFSM().Update();
    }
}
