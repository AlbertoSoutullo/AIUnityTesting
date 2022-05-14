// Unity Imports
using UnityEngine;

// Project Imports
using EnemyAI.Scripts;

public class DoDamage : MonoBehaviour
{
    public int damage = 100;
    private void OnTriggerEnter(Collider other)
    {
        if (other.name != "NewEnemy(Clone)") return;
        
        other.gameObject.GetComponent<Enemy>().TakeDamage(damage);
    }
}
