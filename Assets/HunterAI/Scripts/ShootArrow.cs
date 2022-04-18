using System.Collections;
using System.Collections.Generic;
using HunterAI.Scripts;
using UnityEngine;

public class ShootArrow : MonoBehaviour
{
    public GameObject projectile;
    public Transform bow_position;
    public float launchVelocity = 700f;

    private CompanionMovement fsm;
    
    public void Shoot()
    {
        GameObject arrow = Instantiate(projectile, bow_position.position,  
            transform.rotation);
        
        fsm = GetComponent<CompanionMovement>();
        Vector3 target = fsm.currentTarget.transform.position - transform.position;
        
        arrow.GetComponent<Rigidbody>().AddForce(target * launchVelocity);

        // Physics.IgnoreCollision(arrow.GetComponent<Collider>(), GetComponent<Collider>());
    }
}
