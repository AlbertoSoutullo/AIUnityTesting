using UnityEngine;
using UnityEngine.AI;

namespace EnemyAI.New_Enemy.Scripts
{
    public class Enemy : MonoBehaviour
    {
        public int enemyHp = 100;
        public int attackDamage = 20;
        public GameObject arrowToSpawn;
        
        private Animator _animator;
        private NavMeshAgent _navMeshAgent;
        
        private static readonly int IsDead = Animator.StringToHash("isDead");
        private static readonly int Damaged = Animator.StringToHash("Damaged"); 

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void TakeDamageFromArrow(int damageAmount)
        {
            enemyHp -= damageAmount;
            
            if(enemyHp <= 0)
            {
                GetComponent<CapsuleCollider>().enabled = false;
                SpawnArrows();
                _animator.SetBool(IsDead, true);
                // GetComponent<CapsuleCollider>().enabled = false;
            }
            else
            {
                _animator.SetTrigger(Damaged);
            }
        }

        public void DoDamage()
        {
            var boxCollider = GetComponent<BoxCollider>();
            boxCollider.enabled = true;

            Collider[] cols = Physics.OverlapBox(boxCollider.bounds.center, boxCollider.bounds.extents, boxCollider.transform.rotation);

            foreach (Collider c in cols)
            {
                if (c.gameObject.name == "RedRidingHood")
                {
                    c.gameObject.GetComponent<Player>().TakeDamage(attackDamage);
                }
            }

            boxCollider.enabled = false;
        }

        public void DeathTrigger()
        {
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.name != "Arrow(Clone)") return;
            if (enemyHp < 0) return;
            
            TakeDamageFromArrow(100);
            _navMeshAgent.speed /= 2;
        }

        private void SpawnArrows()
        {
            Vector3 newPosition = transform.root.position;
            Debug.Log($"in spawn: {newPosition}");
            //newPosition.y = 1.5f;
            
            Instantiate(arrowToSpawn, transform.position, Quaternion.identity);
            Debug.Log($"after spawn: {transform.position}");
        }
    
    }
}
