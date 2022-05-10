// Unity Imports
using System;
using UnityEngine;

// Project Imports
using HunterAI.Scripts;

public class Player: MonoBehaviour
{
    public float health, maxHealth = 100;
    public HealthBar healthBar;

    private GameObject _hunter;
    
    public static event Action<Player> InstanceStarted;

    public void TakeDamage(int damage){
        health -= Mathf.Min( damage, health / 4f );            
        healthBar.UpdateHealthBar();
    }
    
    void Start(){
        NotifyPlayerHasBeenInstantiated();
        ObserveForHunterInstantiation();
    }

    private void ObserveForHunterInstantiation()
    {
        CompanionMovement.InstanceStarted += OnHunterInstanceStarted;
    }

    private void OnHunterInstanceStarted(CompanionMovement instance)
    {
        CompanionMovement.InstanceStarted -= OnHunterInstanceStarted;
        _hunter = instance.gameObject;
    }

    private void NotifyPlayerHasBeenInstantiated()
    {
        InstanceStarted?.Invoke(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "ArrowSpawned(Clone)")
        {
            _hunter.GetComponent<CompanionMovement>().arrows += 1;
            Destroy(other.gameObject);
        }
    }
}
