using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }
    void Awake() => Inst = this;

    public GameObject[] enemyObjs;
    public Transform[] spawnPoints;

    public float maxSpawnDelay;
    public float curSpawnDelay;
    public Player player;

    private void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if (curSpawnDelay > maxSpawnDelay)
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

        var enemyObj = Instantiate(enemyObjs[randEnemy], spawnPoints[randPoint].position, spawnPoints[randPoint].rotation);
        var rigid = enemyObj.GetComponent<Rigidbody2D>();
        var enemy = enemyObj.GetComponent<Enemy>();

        if (randPoint == 5 || randPoint == 6)
        {
            enemyObj.transform.Rotate(Vector3.back * 90.0f);
            rigid.velocity = new Vector2(enemy.speed * -1.0f, -1.0f);
        }
        else if (randPoint == 7 || randPoint == 8)
        {
            enemyObj.transform.Rotate(Vector3.forward * 90.0f);
            rigid.velocity = new Vector2(enemy.speed, -1.0f);
        }
        else
        {
            rigid.velocity = new Vector2(0.0f, enemy.speed * -1.0f);
        }
    }

    public void PlayerHit()
    {
        player.gameObject.SetActive(true);

        Invoke(nameof(RespawnPlayerExe), 2.0f);
    }
    
    void RespawnPlayerExe()
    {
        player.transform.position = Vector3.down * 3.5f;
        player.gameObject.SetActive(true);
    }
}
