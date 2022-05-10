using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float physicsVelocity = 300;
    public float rotationSpeed = 720;
    public float runSpeedIncrement = 1.5f;
    
    private Rigidbody _rigidbody;
    private Animator _animationController;
    
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsWalking = Animator.StringToHash("isWalking");

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animationController = GetComponent<Animator>();
    }
    
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        bool runPressed = Input.GetKey("left shift");
        
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);
        Vector3 runMovement = new Vector3(moveHorizontal * runSpeedIncrement, 0, moveVertical * runSpeedIncrement);

        if (runPressed)
            Run(runMovement);
        else
            Walk(movement);

        if (PlayerIsMoving(movement))
            RotateTowardsMovement(movement);
        else
            StopWalkingAnimation();
    }

    private void Run(Vector3 runMovement)
    {
        _animationController.SetBool(IsRunning, true);
        Vector3 runMovementWithSpeed = runMovement * (physicsVelocity * Time.deltaTime);
        _rigidbody.velocity = new Vector3(runMovementWithSpeed.x, _rigidbody.velocity.y, runMovementWithSpeed.z);
    }

    private void Walk(Vector3 movement)
    {
        _animationController.SetBool(IsRunning, false);
        Vector3 movementWithSpeed = movement * (physicsVelocity * Time.deltaTime);
        _rigidbody.velocity = new Vector3(movementWithSpeed.x, _rigidbody.velocity.y, movementWithSpeed.z);
    }

    private void RotateTowardsMovement(Vector3 movement)
    {
        Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 
            rotationSpeed * Time.deltaTime);
        _animationController.SetBool(IsWalking, true);
    }

    private bool PlayerIsMoving(Vector3 movement)
    {
        return (movement != Vector3.zero);
    }

    private void StopWalkingAnimation()
    {
        _animationController.SetBool(IsWalking, false);
    }
}
