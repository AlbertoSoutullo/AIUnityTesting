// Unity Imports
using System.Collections;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace Player.Scripts
{
    public class Spawner : MonoBehaviour
    {
        public LayerMask terrainLayer;
        public GameObject enemy;
        public GameObject hunter;

        public float minRadius = 15;
        public float maxRadius = 40;
        public float minSpawnTime = 15;
        public float maxSpawnTime = 30;
    
        private IEnumerator _coroutine;
        private void Start()
        {
            StartCoroutine(SpawnHunter());
            _coroutine = SpawnEnemies();
            StartCoroutine(_coroutine);
        }

        private IEnumerator SpawnHunter()
        {
            float secondsToWait = 5;
            yield return new WaitForSeconds(secondsToWait);

            Vector3 randomPosition = GetSpawnPositionWithingRing();

            randomPosition.y = CalculateHeightValue(randomPosition.x, randomPosition.z);
            randomPosition.y += hunter.transform.position.y / 2;

            Instantiate(hunter, randomPosition, Quaternion.identity);
        }

        private IEnumerator SpawnEnemies()
        {
            while (true)
            {
                float secondsToWait = Random.Range(minSpawnTime, maxSpawnTime);
                yield return new WaitForSeconds(secondsToWait);

                Vector3 randomPosition = GetSpawnPositionWithingRing();

                randomPosition.y = CalculateHeightValue(randomPosition.x, randomPosition.z);
            
                randomPosition.y += enemy.transform.position.y / 2;

                Instantiate(enemy, randomPosition, Quaternion.identity);
            }
        }

        private Vector3 GetSpawnPositionWithingRing()
        {
            float radius = Mathf.Sqrt(Random.Range(minRadius*minRadius, maxRadius*maxRadius));
            float angle = Random.Range(-Mathf.PI, Mathf.PI);
  
            Vector3 centralSpawnPosition = transform.position;
            centralSpawnPosition += new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle)) * radius;

            return centralSpawnPosition;
        }

        private float CalculateHeightValue(float randomPositionx, float randomPositionz)
        {
            float randomPositiony = 0;
            
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(randomPositionx, 9999f, randomPositionz), Vector3.down,
                out hit, Mathf.Infinity, terrainLayer))
            {
                randomPositiony = hit.point.y;
            }

            return randomPositiony;
        }
    }
}
