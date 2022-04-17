using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generateEnemies : MonoBehaviour

{
    public GameObject enemy;
    public int xPos;
    public int zPos;
    public int yPos;
    public int enemyCount;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnemyDrop());
        
    }
    IEnumerator EnemyDrop()
    {
        while (enemyCount < 10)
        {
            xPos = Random.Range(90, 750);
          
            zPos = Random.Range(50, 432);
            Instantiate(enemy, new Vector3(xPos, 1, zPos), Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            enemyCount += 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
