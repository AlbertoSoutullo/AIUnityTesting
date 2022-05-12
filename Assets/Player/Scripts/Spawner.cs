// Unity Imports
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player.Scripts
{
    public class Spawner : MonoBehaviour
    {
        public LayerMask terrainLayer;
        public GameObject enemy;
        public GameObject hunter;
    
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
        
            var position = transform.position;
            float randomPositiony = 0;
            float randomPositionx = Random.Range(position.x, position.x + maxRadius);
            float randomPositionz = Random.Range(position.z, position.z + maxRadius);

            RaycastHit hit;
            if (Physics.Raycast(new Vector3(randomPositionx, 9999f, randomPositionz), Vector3.down,
                out hit, Mathf.Infinity, terrainLayer))
            {
                randomPositiony = hit.point.y;
            }
            
            randomPositiony += hunter.transform.position.y / 2;
            Vector3 randomPosition = new Vector3(randomPositionx, randomPositiony, randomPositionz);

            Instantiate(hunter, randomPosition, Quaternion.identity);
        }

        private IEnumerator SpawnEnemies()
        {
            while (true)
            {
                float secondsToWait = Random.Range(minSpawnTime, maxSpawnTime);
                yield return new WaitForSeconds(secondsToWait);

                var transform1 = transform;
                float randomPositiony = 0;
                float randomPositionx = Random.Range(transform1.position.x, transform1.position.x + maxRadius);
                float randomPositionz = Random.Range(transform.position.z, transform.position.z + maxRadius);

                RaycastHit hit;
                if (Physics.Raycast(new Vector3(randomPositionx, 9999f, randomPositionz), Vector3.down,
                    out hit, Mathf.Infinity, terrainLayer))
                {
                    randomPositiony = hit.point.y;
                }
            
                randomPositiony += enemy.transform.position.y / 2;
                Vector3 randomPosition = new Vector3(randomPositionx, randomPositiony, randomPositionz);

                Instantiate(enemy, randomPosition, Quaternion.identity);
            }
        }
    }
}
