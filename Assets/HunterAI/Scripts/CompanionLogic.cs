// Unity Imports
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HunterAI.Scripts
{
    public class CompanionLogic: MonoBehaviour
    {
        private Transform _playerTransform;
        
        public float weaponRangeDistance = 8;
        public float visionRangeDistance = 25;
        public float radius = 5.0f;
        public float playerMaxDistanceToRun = 10f;
        
        void Start()
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        public IEnumerable<GameObject> EnemiesThatCanBeAttacked()
        {
            return GameObject.FindGameObjectsWithTag("Enemy")
                .Where(enemy => Vector3.Distance(transform.position, enemy.transform.position) < visionRangeDistance
                                && Vector3.Distance(_playerTransform.position, enemy.transform.position) < (weaponRangeDistance + playerMaxDistanceToRun));
        }

        private IEnumerable<GameObject> ExistingArrows()
        {
            return GameObject.FindGameObjectsWithTag("SpawnedArrow")
                .Where(arrow => Vector3.Distance(transform.position, arrow.transform.position) < visionRangeDistance);
        }
        
        public float DistanceWithPlayer()
        {
            return Vector3.Distance(transform.position, _playerTransform.position);
        }
        
        public GameObject GetClosestEnemy()
        {
            IEnumerable<GameObject> closeEnemies = EnemiesThatCanBeAttacked();
            GameObject targetEnemy = null;
            float minimumDistance = float.MaxValue;

            foreach (GameObject enemy in closeEnemies)
            {
                float distanceToPlayer = Vector3.Distance(enemy.transform.position, _playerTransform.position);
                if (distanceToPlayer < minimumDistance)
                {
                    minimumDistance = distanceToPlayer;
                    targetEnemy = enemy;
                }
            }
            return targetEnemy;
        }
        
        public GameObject GetClosestArrow()
        {
            IEnumerable<GameObject> arrows = ExistingArrows();
            GameObject targetArrow = null;
            float minimumDistance = float.MaxValue;

            foreach (GameObject arrow in arrows)
            {
                float distanceToPlayer = Vector3.Distance(arrow.transform.position, _playerTransform.position);
                if (distanceToPlayer < minimumDistance)
                {
                    minimumDistance = distanceToPlayer;
                    targetArrow = arrow;
                }
            }
            return targetArrow;
        }
        
        public Vector3 GetRandomPositionWithinPlayerRange()
        {
            Vector3 playerPositon = _playerTransform.position;
            float angle = Random.Range(0, 2f * Mathf.PI);
			
            return playerPositon + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
        }
    }
}