using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }
    void Awake() => Inst = this;

    //---------------------------------------------------
    public Transform[] spawnPoints;

    public float maxSpawnDelay;
    public float curSpawnDelay;
    //---------------------------------------------------
    public Player player;
    //---------------------------------------------------
    public Text scoreText;
    public Image[] lifeImage;
    public Image[] bombImage;

    public GameObject gameOverSet;
    public GameObject bombEffect;
    //---------------------------------------------------


    public int score;
    int playerLife = 3;




    void Start()
    {
        UpdateLifeIcon();
        UpdateBombIcon();
    }

    private void Update()
    {
        scoreText.text = score.ToString("#,##0");

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
        int randEnemy = Random.Range(0, 3);
        int randPoint = Random.Range(0, spawnPoints.Length);

        PoolType poolType;
        if ( randEnemy == 0 )
            poolType = PoolType.enemyL;
        else if(randEnemy == 1)
            poolType = PoolType.enemyM;
        else
            poolType = PoolType.enemyS;

        var enemyObj = ObjectManager.Inst.MakeObj(poolType);
        enemyObj.transform.position = spawnPoints[randPoint].position;
        enemyObj.transform.rotation = spawnPoints[randPoint].rotation;

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

    public void OnClick_Retry()
    {
        SceneManager.LoadScene(0);
    }


    public void PlayerHit()
    {
        player.gameObject.SetActive(false);
        playerLife -= 1;
        UpdateLifeIcon();

        if (playerLife <= 0)
        {
            GameOver();
        }
        else
        {
            Invoke(nameof(RespawnPlayerExe), 2.0f);
        }
    }
    
    void RespawnPlayerExe()
    {
        player.transform.position = Vector3.down * 3.5f;
        player.gameObject.SetActive(true);
    }

    void UpdateLifeIcon()
    {
        for( int i = 0; i < lifeImage.Length; i++ )
        {
            lifeImage[i].color = Color.gray;
        }

        for (int i = 0; i < playerLife; i++)
        {
            lifeImage[i].color = Color.white;
        }
    }

    void GameOver()
    {
        gameOverSet.SetActive(true);
    }

    uint curBombCnt = 1;
    uint MAX_BOMB = 3;
    bool IsBombTime = false;

    public void AddBomb(uint addCnt)
    {
        if ( curBombCnt != MAX_BOMB)
        {
            curBombCnt += addCnt;
            UpdateBombIcon();
        }
        else
        {
            score += 500;
        }
    }

    public void ExecuteBomb()
    {
        if (IsBombTime)
            return;

        if (curBombCnt <= 0)
            return;

        curBombCnt--;
        UpdateBombIcon();

        IsBombTime = true;
        bombEffect.SetActive(true);

        var objList = new List<GameObject>();

        objList.AddRange(ObjectManager.Inst.GetPoolList(PoolType.enemyL));
        objList.AddRange(ObjectManager.Inst.GetPoolList(PoolType.enemyM));
        objList.AddRange(ObjectManager.Inst.GetPoolList(PoolType.enemyS));
        foreach (var item in objList)
        {
            if( item.activeSelf )
                item.GetComponent<Enemy>().OnHit(1000);
        }
        objList.Clear();

        objList.AddRange(ObjectManager.Inst.GetPoolList(PoolType.bulletEnemyA));
        objList.AddRange(ObjectManager.Inst.GetPoolList(PoolType.bulletEnemyB));
        foreach (var item in objList)
        {
            if (item.activeSelf)
                item.gameObject.SetActive(false);
        }

        Invoke(nameof(OffBombEffect), 3.0f);
    }

    void OffBombEffect()
    {
        IsBombTime = false;
        bombEffect.SetActive(false);
    }



    void UpdateBombIcon()
    {
        for (int i = 0; i < bombImage.Length; i++)
        {
            bombImage[i].color = Color.gray;
        }

        for (int i = 0; i < curBombCnt; i++)
        {
            bombImage[i].color = Color.white;
        }
    }
}
