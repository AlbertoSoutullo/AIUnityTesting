// Unity Imports
using UnityEngine;

// Project Imports
using Player.Scripts;


namespace EnemyAI.Scripts
{
    public class Enemy : MonoBehaviour
    {
        public int enemyHp = 100;
        public int attackDamage = 20;
        public GameObject arrowToSpawn;
        
        private Animator _animator;

        private static readonly int IsDead = Animator.StringToHash("isDead");
        private static readonly int Damaged = Animator.StringToHash("Damaged"); 

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void TakeDamage(int damageAmount)
        {
            enemyHp -= damageAmount;
            
            if(IsEnemyDead())
            {
                GetComponent<CapsuleCollider>().enabled = false;
                SpawnArrow();
                _animator.SetBool(IsDead, true);
            }
            else
                _animator.SetTrigger(Damaged);
        }

        private bool IsEnemyDead()
        {
            return (enemyHp <= 0);
        }

        public void DoDamageAnimationEvent()
        {
            BoxCollider boxCollider = EnableHitBox();
            Collider[] cols = DetectHits(boxCollider);
            ApplyDamage(cols);

            boxCollider.enabled = false;
        }
        
        private BoxCollider EnableHitBox()
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            boxCollider.enabled = true;

            return boxCollider;
        }

        private Collider[] DetectHits(BoxCollider boxCollider)
        {
            var bounds = boxCollider.bounds;
            Collider[] cols = Physics.OverlapBox(bounds.center, bounds.extents, 
                boxCollider.transform.rotation);

            return cols;
        }

        private void ApplyDamage(Collider[] colliders)
        {
            foreach (Collider c in colliders)
            {
                if (c.gameObject.name == "child")
                    c.gameObject.GetComponent<PlayerScript>().TakeDamage(attackDamage);
            }
        }

        private void SpawnArrow()
        {
            Instantiate(arrowToSpawn, transform.position, Quaternion.identity);
        }
    }
}
