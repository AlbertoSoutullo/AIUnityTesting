using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnEnemies : MonoBehaviour
{
    public LayerMask TerrainLayer;
    public GameObject Enemy;
    
    public float maxRadious = 40;
    public float minSpawnTime = 15;
    public float maxSpawnTime = 30;
    

    private IEnumerator coroutine;
    private void Start()
    {
        coroutine = Spawn();
        StartCoroutine(coroutine);
    }

    private IEnumerator Spawn()
    {
        while (true)
        {
            
            float secondsToWait = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(secondsToWait);

            float randomPositiony = 0;
            float randomPositionx = Random.Range(transform.position.x, transform.position.x + maxRadious);
            float randomPositionz = Random.Range(transform.position.z, transform.position.z + maxRadious);

            RaycastHit hit;
            if (Physics.Raycast(new Vector3(randomPositionx, 9999f, randomPositionz), Vector3.down,
                out hit, Mathf.Infinity, TerrainLayer))
            {
                randomPositiony = hit.point.y;
            }

            randomPositiony += Enemy.GetComponent<CapsuleCollider>().height / 2;
            Vector3 randomPosition = new Vector3(randomPositionx, randomPositiony, randomPositionz);

            Instantiate(Enemy, randomPosition, Quaternion.identity);
        }

    }
}
