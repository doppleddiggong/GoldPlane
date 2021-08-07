using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }

    //---------------------------------------------------
    public Transform[] spawnPoints;

    float nextSpawnDelay;
    float curSpawnDelay;
    //---------------------------------------------------
    public Player player;
    //---------------------------------------------------
    public Text scoreText;
    public Image[] lifeImage;
    public Image[] bombImage;

    public GameObject gameOverSet;
    public GameObject bombEffect;
    //---------------------------------------------------


    [HideInInspector] public int score;
    static int MAX_PLAYER_LIFE = 3;
    int playerLife = MAX_PLAYER_LIFE;

    void Awake()
    {
        Inst = this;

        ReadSpawnFile();

        UpdateLifeIcon();
        UpdateBombIcon();
    }

    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if (curSpawnDelay > nextSpawnDelay && spawnEnd == false )
        {
            SpawnEnemy();
            curSpawnDelay = 0.0f;
        }

        scoreText.text = score.ToString("#,##0");
    }

    void SpawnEnemy()
    {
        PoolType poolType = PoolType.enemyS;
        if (spawnList[spawnIndex].type == "L" )
            poolType = PoolType.enemyL;
        else if (spawnList[spawnIndex].type == "M")
            poolType = PoolType.enemyM;
        else if (spawnList[spawnIndex].type == "S")
            poolType = PoolType.enemyS;

        int enemyPoint = spawnList[spawnIndex].point;

        var enemyObj = ObjectManager.Inst.MakeObj(poolType);
        enemyObj.transform.position = spawnPoints[enemyPoint].position;
        enemyObj.transform.rotation = spawnPoints[enemyPoint].rotation;

        var rigid = enemyObj.GetComponent<Rigidbody2D>();
        var enemy = enemyObj.GetComponent<Enemy>();

        if (enemyPoint == 5 || enemyPoint == 6)
        {
            enemyObj.transform.Rotate(Vector3.back * 90.0f);
            rigid.velocity = new Vector2(enemy.speed * -1.0f, -1.0f);
        }
        else if (enemyPoint == 7 || enemyPoint == 8)
        {
            enemyObj.transform.Rotate(Vector3.forward * 90.0f);
            rigid.velocity = new Vector2(enemy.speed, -1.0f);
        }
        else
        {
            rigid.velocity = new Vector2(0.0f, enemy.speed * -1.0f);
        }

        // #.리스폰 인덱스 증가
        spawnIndex++;
        if (spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            return;
        }

        // #.다음 리스폰 딜레이 갱신
        nextSpawnDelay = spawnList[spawnIndex].delay;
    }

    public void OnClick_Retry()
    {
        SceneManager.LoadScene(0);
    }


    public void PlayerHit()
    {
        player.gameObject.SetActive(false);
        player.FollowerActivate(false);

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
        player.FollowerActivate(true);
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



    List<Spawn> spawnList = new List<Spawn>();
    int spawnIndex;
    bool spawnEnd;

    void ReadSpawnFile()
    {
        // #1.변수 초기화
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        // #2.리스폰 파일 읽기
        var textFile = Resources.Load("Stage0") as TextAsset;
        var reader = new StringReader(textFile.text);
        // #. 첫 줄은 넘긴다
        reader.ReadLine();

        // #3. 한줄씩 데이터 저장
        while (reader != null)
        {
            string line = reader.ReadLine();

            if (line == null)
                break;

            var data = new Spawn();

            string[] v = line.Split(',');
            data.delay = float.Parse(v[0]);
            data.type = v[1];
            data.point = int.Parse(v[2]);

            spawnList.Add(data);
        }
        // #4.텍스트 파일 닫기
        reader.Close();

        // #. 첫번쨰 스폰 딜레이
        nextSpawnDelay = spawnList[0].delay;
    }

}
