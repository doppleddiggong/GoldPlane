using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyObjs;
    public Transform[] spawnPoints;

    public float maxSpawnDelay;
    public float curSpawnDelay;


    private void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if( curSpawnDelay > maxSpawnDelay )
        {
            SpawnEnemy();

            maxSpawnDelay = Random.Range(0.5f, 3.0f);
            curSpawnDelay = 0.0f;
        }
    }

    void SpawnEnemy()
    {
        int randEnemy = Random.Range(0, enemyObjs.Length);
        int randPoint = Random.Range(0, spawnPoints.Length);

        Instantiate(enemyObjs[randEnemy], spawnPoints[randPoint].position, spawnPoints[randPoint].rotation);
    }
}
