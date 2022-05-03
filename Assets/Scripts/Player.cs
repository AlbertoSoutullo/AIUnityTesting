using System;
using System.Collections;
using System.Collections.Generic;
using HunterAI.Scripts;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public class Player: MonoBehaviour
{
    public float health, maxHealth;
    public HealthBar healthBar;

    private GameObject hunter;
    
    public static event Action<Player> InstanceStarted;

    public void TakeDamage(int damage){
        // Use your own damage handling code, or this example one.
        health -= Mathf.Min( damage, health / 4f );            
        healthBar.UpdateHealthBar();
    }
    
    void Start(){
        hunter = GameObject.FindGameObjectWithTag("Hunter");
        InstanceStarted?.Invoke(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "ArrowSpawned(Clone)")
        {
            hunter.GetComponent<CompanionMovement>().arrows += 1;
            Destroy(other.gameObject);
        }
    }
    
}
