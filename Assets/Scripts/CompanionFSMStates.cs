using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class IdleState : FSMState<CompanionMovement>
{
	static readonly IdleState instance = new IdleState();
	public static IdleState Instance { get { return instance; } }

	static IdleState()
	{
	}
	IdleState()
	{
	}

	public override void Enter(CompanionMovement companion)
	{
		Debug.Log("Entering IdleState");
	}

	public override void Execute(CompanionMovement companion)
	{
		// First of all stop any possible ongoing walking animation
		companion.StopWalking();
		Transform player = companion.player;
		// If too far from player, follow the player
		if (companion.DistanceWithPlayer() >= companion.playerMaxDistance)
		{
			Debug.Log("Too far from player so changing to FollowPlayerState");
			companion.GetFSM().ChangeState(FollowPlayerState.Instance);
		}
		// If companion does not have any arrows, point to closest
		else if (companion.arrows < 1)
		{
			Debug.Log("No arrows.");
			companion.GetFSM().ChangeState(PointState.Instance);
		}
		// If any enemy can be attacked, go to attacking mode
		else if (companion.EnemiesThatCanBeAttacked().Count() > 0)
        {
			Debug.Log("An enemy can be attacked so changing to FollowPlayerState");
			companion.GetFSM().ChangeState(ChooseTargetState.Instance);
		}

    }

	public override void Exit(CompanionMovement companion)
	{
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}

public class FollowPlayerState : FSMState<CompanionMovement>
{
	static readonly FollowPlayerState instance = new FollowPlayerState();
	public static FollowPlayerState Instance { get { return instance; } }

	static FollowPlayerState()
	{
	}
	FollowPlayerState()
	{
	}

	public override void Enter(CompanionMovement companion)
	{
		Debug.Log("Entering FollowPlayerState");
	}

	public override void Execute(CompanionMovement companion)
	{
		// If further than companion.playerMaxDistance from the player,
		// just move towards the player
		Transform player = companion.player;
		if (companion.DistanceWithPlayer() >= companion.playerMaxDistance)
			companion.WalkTo(player.position);
		// Whenever close enough to the player, go back to idle
		else
		{
			Debug.Log("Close enough to player so changing to IdleState");
			companion.GetFSM().ChangeState(IdleState.Instance);
		}
	}

	public override void Exit(CompanionMovement companion)
	{
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}

public class PointState : FSMState<CompanionMovement>
{
	static readonly PointState instance = new PointState();
	public static PointState Instance { get { return instance; } }

	static PointState()
	{
	}
	PointState()
	{
	}

	public override void Enter(CompanionMovement companion)
	{
		Debug.Log("Entering PointState");
		GameObject arrow = companion.GetClosestArrow();
		if (arrow != null)
		{
			companion.transform.LookAt(arrow.transform.position);
			companion.Point();
		}
		else
			companion.GetFSM().ChangeState(IdleState.Instance);
	}

	public override void Execute(CompanionMovement companion)
	{

		//Debug.Log("Going back to IdleState");
		//companion.GetFSM().ChangeState(IdleState.Instance);
	}

	public override void Exit(CompanionMovement companion)
	{
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}

public class ChooseTargetState : FSMState<CompanionMovement>
{
	static readonly ChooseTargetState instance = new ChooseTargetState();
	public static ChooseTargetState Instance { get { return instance; } }

	static ChooseTargetState()
	{
	}
	ChooseTargetState()
	{
	}

	public override void Enter(CompanionMovement companion)
	{
		Debug.Log("Entering ChooseTargetState");
	}

	public override void Execute(CompanionMovement companion)
	{
		// From those enemies that the companion can see,
		// select the closest to the player
		companion.currentTarget = companion.GetClosestEnemy();

		// If the companion is already in weapon range
		// with target, load the bow
		if (Vector3.Distance(companion.currentTarget.transform.position, companion.transform.position) < companion.weaponRangeDistance)
		{
			Debug.Log("An enemy is close enough so changing to AttackState");
			companion.GetFSM().ChangeState(AttackState.Instance);
		}
		// Otherwise, move close enough to target and then
		// load the bow
		else
		{
			Debug.Log("Too far from enemy so changing to RelocateState");
			companion.GetFSM().ChangeState(RelocateState.Instance);
		}
	}

	public override void Exit(CompanionMovement companion)
	{
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}

public class RechargeState : FSMState<CompanionMovement>
{
	static readonly RechargeState instance = new RechargeState();
	public static RechargeState Instance { get { return instance; } }

	static RechargeState()
	{
	}
	RechargeState()
	{
	}

	public override void Enter(CompanionMovement companion)
	{
		Debug.Log("Entering RechargeState");
	}

	public override void Execute(CompanionMovement companion)
	{
		// If the companion's weapon is not charged,
        // charge it then attack
		if (!companion.weaponIsCharged)
		{
			Debug.Log("Charging weapon");
			companion.weaponIsCharged = true;
		}
		Debug.Log("Weapon is charged so changing to AttackState");
		companion.GetFSM().ChangeState(AttackState.Instance);

	}

	public override void Exit(CompanionMovement companion)
	{
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}

public class RelocateState : FSMState<CompanionMovement>
{
	static readonly RelocateState instance = new RelocateState();
	public static RelocateState Instance { get { return instance; } }

	private bool desiredLocationSet;
	private Vector3 desiredLocation;

	static RelocateState()
	{
	}
	RelocateState()
	{
	}

	public override void Enter(CompanionMovement companion)
	{
		Debug.Log("Entering RelocateState");
		desiredLocationSet = false;
	}

	public override void Execute(CompanionMovement companion)
	{
		// First of all compute the location to go
		// (close enough to both player and target)
		if (!desiredLocationSet)
        {
			Debug.Log("Computing location to attack current target while within player's range");
			desiredLocation =
				companion.player.transform.position
				+ (companion.currentTarget.transform.position - companion.player.transform.position)
				  * companion.playerMaxDistance / (companion.playerMaxDistance + companion.weaponRangeDistance);
			desiredLocationSet = true;
		}
		// Then move to said location and attack
		else
			companion.WalkTo(desiredLocation);
		if (Vector3.Distance(desiredLocation, companion.transform.position) < 1)
		{
			Debug.Log("Arrived in desired location so changing to AttackState");
			companion.StopWalking();
			companion.GetFSM().ChangeState(AttackState.Instance);
		}


	}

	public override void Exit(CompanionMovement companion)
	{
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}

public class AttackState : FSMState<CompanionMovement>
{
	static readonly AttackState instance = new AttackState();
	public static AttackState Instance { get { return instance; } }

	private bool desiredLocationSet;
	private Vector3 desiredLocation;

	static AttackState()
	{
	}
	AttackState()
	{
	}
	public override void Enter(CompanionMovement companion)
	{
		Debug.Log("Entering AttackState");
		// First of all attack the target
		companion.Attack();
	}

	public override void Execute(CompanionMovement companion)
	{
		if (companion.currentTarget != null)
			companion.transform.LookAt(companion.currentTarget.transform.position);
		//if (companion.IsAttackAnimationFinished())
		//{
		//	companion.FinishAttack();

		//	// If no enemy can be attacked anymore, go back to idle
		//	GameObject currentClosestEnemy = companion.GetClosestEnemy();
		//	if (currentClosestEnemy == null)
		//	{
		//		Debug.Log("No enemies can be attacked anymore so changing to IdleState");
		//		companion.GetFSM().ChangeState(IdleState.Instance);
		//	}
		//	// If another enemy is closer, change target
		//	else if (currentClosestEnemy != companion.currentTarget)
		//	{
		//		Debug.Log("Current target is no longer the closest so changing to ChooseTargetState");
		//		companion.GetFSM().ChangeState(ChooseTargetState.Instance);

		//	}
		//	else
		//	{
		//		// If not close enough (I think it's impossible though)
		//		// relocate again
		//		if (Vector3.Distance(companion.transform.position, companion.currentTarget.transform.position) > companion.weaponRangeDistance)
		//		{
		//			Debug.Log("No longer in current target's range so changing to RelocateState");
		//			companion.GetFSM().ChangeState(RelocateState.Instance);
		//		}
		//		else
		//			companion.GetFSM().ChangeState(AttackState.Instance);
		//		// Otherwise recharge and attack again
		//		// else
		//		// {
		//		// Debug.Log("In current target's range so keeping in AttackState");
		//		// this.GetFSM().ChangeState(AttackState.Instance);
		//		// }
		//	}
		//}
		//else if (companion.currentTarget != null)
		//	companion.transform.LookAt(companion.currentTarget.transform.position);
	}

	public override void Exit(CompanionMovement companion)
	{
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}