using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateControllerPlayer : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;
    int isRunningHash;
    Transform player;
    // another animation to go everywhere
    public float playerSpeed = 10.0f;
    public float playerRotationSpeed = 100.0f;

    public GameObject healthbar;
    public float health = 10;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");

    }

    // Update is called once per frame
    void Update()
    {
        bool isrunning = animator.GetBool(isRunningHash);
        bool isWalking = animator.GetBool(isWalkingHash);
        bool forwardPressed = Input.GetKey("w");
        bool runPressed = Input.GetKey("left shift");
        //if player presses w key
        if (!isWalking && forwardPressed)
        {// then set the isWalking boolean to be true 
            animator.SetBool(isWalkingHash, true);
        }
        //if player is not pressing w key
        if (isWalking && !forwardPressed)
        {  //then set the isWalking boolean to be false

            animator.SetBool(isWalkingHash, false);
        }
        //if player is walking and not running press left shift
        if (!isrunning && (forwardPressed && runPressed))
        {
            animator.SetBool(isRunningHash, true);
        }
        //if player is reunning ans stop running or stop walking
        if (isrunning && (!forwardPressed || !runPressed))
        {
            animator.SetBool(isRunningHash, false);
        }

        Debug.Log(isWalking.ToString());
        Debug.Log(isrunning.ToString());
        //Get the horizental and vertical axis using arrow keys
        // the valie is in the range -1 to 1
        //Make it move 10 meters per second instead of 10 meters per frame
        float translation = Input.GetAxis("Vertical") * playerSpeed * Time.deltaTime;
        float rotation = Input.GetAxis("Horizontal") * playerRotationSpeed * Time.deltaTime;
        // Move translation along the object's z-axis
        transform.Translate(0, 0, translation);
        //Rotate around our y-axis
        transform.Rotate(0, rotation, 0);

        player = GameObject.FindGameObjectWithTag("Player").transform;
       // healthbar.transform.localScale.x = health;


    }
   /* void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Enemy")
        {
            healthbar.health -= 1;
            Debug.Log(health);

        }
    }*/
}

