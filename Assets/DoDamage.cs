using EnemyAI.Scripts;
using UnityEngine;

public class DoDamage : MonoBehaviour
{
    public int damage = 100;
    private void OnTriggerEnter(Collider other)
    {
        if (other.name != "NewEnemy(Clone)") return;
        
        other.gameObject.GetComponent<Enemy>().TakeDamage(damage);
    }
}
