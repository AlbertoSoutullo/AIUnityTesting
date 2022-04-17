using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int enemyHP = 100;
    public GameObject projectile;
    public Transform projectilePoint;

    public Animator animator;

    public GameObject arrowToSpawn;
    private NavMeshAgent _navMeshAgent;
    
    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void Shoot()
    {
        Rigidbody rb = Instantiate(projectile, projectilePoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 30f, ForceMode.Impulse);
        rb.AddForce(transform.up * 7, ForceMode.Impulse);
    }

    public void TakeDamageFromArrow(int damageAmount)
    {
        enemyHP -= damageAmount;
        if(enemyHP <= 0)
        {
            animator.SetTrigger("Death");
            // GetComponent<CapsuleCollider>().enabled = false;
            spawnArrows();
            Destroy(gameObject);
        }
        else
        {
            animator.SetTrigger("Damage");
        }
    }

    public void DoDamage()
    {
        var boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = true;

        Collider[] cols = Physics.OverlapBox(boxCollider.bounds.center, boxCollider.bounds.extents, boxCollider.transform.rotation);

        foreach (Collider c in cols)
        {
            if (c.gameObject.name == "RedRidingHood(Clone)")
            {
                c.gameObject.GetComponent<Player>().TakeDamage(20);
            }
        }

        boxCollider.enabled = false;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Arrow(Clone)")
        {
            TakeDamageFromArrow(50);
            _navMeshAgent.speed /= 2;
            Debug.Log("HIT");
        }
    }

    private void spawnArrows()
    {
        Instantiate(arrowToSpawn, transform.position, transform.rotation);
    }
    
}
