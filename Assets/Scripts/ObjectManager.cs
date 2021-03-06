using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolType
{
    enemyL,
    enemyM,
    enemyS,
    enemyBoss,

    follower,

    itemCoin,
    itemPower,
    itemBomb,

    bulletPlayerA,
    bulletPlayerB,
    bulletEnemyA,
    bulletEnemyB,
    bulletFollowerA,
    bulletEnemyBossA,
    bulletEnemyBossB,

    explosion,


    MAX
};

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Inst { get; private set; }

    [SerializeField] GameObject base_enemyL;
    [SerializeField] GameObject base_enemyM;
    [SerializeField] GameObject base_enemyS;
    [SerializeField] GameObject base_enemyBoss;
    [SerializeField] GameObject base_follower;

    [SerializeField] GameObject base_itemCoin;
    [SerializeField] GameObject base_itemPower;
    [SerializeField] GameObject base_itemBomb;

    [SerializeField] GameObject base_bulletPlayerA;
    [SerializeField] GameObject base_bulletPlayerB;
    [SerializeField] GameObject base_bulletEnemyA;
    [SerializeField] GameObject base_bulletEnemyB;
    [SerializeField] GameObject base_bulletFollowerA;
    [SerializeField] GameObject base_bulletEnemyBossA;
    [SerializeField] GameObject base_bulletEnemyBossB;

    [SerializeField] GameObject base_explosion;

    List<GameObject> pool_enemyL = new List<GameObject>();
    List<GameObject> pool_enemyM = new List<GameObject>();
    List<GameObject> pool_enemyS = new List<GameObject>();
    List<GameObject> pool_enemyBoss = new List<GameObject>();
    List<GameObject> pool_follower = new List<GameObject>();

    List<GameObject> pool_itemCoin = new List<GameObject>();
    List<GameObject> pool_itemPower = new List<GameObject>();
    List<GameObject> pool_itemBomb = new List<GameObject>();

    List<GameObject> pool_bulletPlayerA = new List<GameObject>();
    List<GameObject> pool_bulletPlayerB = new List<GameObject>();
    List<GameObject> pool_bulletEnemyA = new List<GameObject>();
    List<GameObject> pool_bulletEnemyB = new List<GameObject>();
    List<GameObject> pool_bulletFollowerA = new List<GameObject>();
    List<GameObject> pool_bulletEnemyBossA = new List<GameObject>();
    List<GameObject> pool_bulletEnemyBossB = new List<GameObject>();

    List<GameObject> pool_explosion = new List<GameObject>();


    void Awake()
    {
        Inst = this;
        Generate();
    }

    void Generate()
    {
        pool_enemyBoss.Add(Instantiate(base_enemyBoss, this.gameObject.transform));
        ObjectAllHide(pool_enemyBoss);

        for (int i = 0; i < 5; i++)
        {
            pool_enemyL.Add(Instantiate(base_enemyL, this.gameObject.transform));
            pool_enemyM.Add(Instantiate(base_enemyM, this.gameObject.transform));
            pool_enemyS.Add(Instantiate(base_enemyS, this.gameObject.transform));
            pool_follower.Add(Instantiate(base_follower, this.gameObject.transform));

            pool_itemCoin.Add(Instantiate(base_itemCoin, this.gameObject.transform));
            pool_itemPower.Add(Instantiate(base_itemPower, this.gameObject.transform));
            pool_itemBomb.Add(Instantiate(base_itemBomb, this.gameObject.transform));

            pool_explosion.Add(Instantiate(base_explosion, this.gameObject.transform));

        }

        ObjectAllHide(pool_enemyL);
        ObjectAllHide(pool_enemyM);
        ObjectAllHide(pool_enemyS);
        ObjectAllHide(pool_follower);

        ObjectAllHide(pool_itemCoin);
        ObjectAllHide(pool_itemPower);
        ObjectAllHide(pool_itemBomb);

        ObjectAllHide(pool_explosion);

        for (int i = 0; i < 10; i++)
        {
            pool_bulletPlayerA.Add(Instantiate(base_bulletPlayerA, this.gameObject.transform));
            pool_bulletPlayerB.Add(Instantiate(base_bulletPlayerB, this.gameObject.transform));

            pool_bulletEnemyA.Add(Instantiate(base_bulletEnemyA, this.gameObject.transform));
            pool_bulletEnemyB.Add(Instantiate(base_bulletEnemyB, this.gameObject.transform));
            pool_bulletFollowerA.Add(Instantiate(base_bulletFollowerA, this.gameObject.transform));

            pool_bulletEnemyBossA.Add(Instantiate(base_bulletEnemyBossA, this.gameObject.transform));
            pool_bulletEnemyBossB.Add(Instantiate(base_bulletEnemyBossB, this.gameObject.transform));
        }

        ObjectAllHide(pool_bulletPlayerA);
        ObjectAllHide(pool_bulletPlayerB);

        ObjectAllHide(pool_bulletEnemyA);
        ObjectAllHide(pool_bulletEnemyB);
        ObjectAllHide(pool_bulletFollowerA);
        ObjectAllHide(pool_bulletEnemyBossA);
        ObjectAllHide(pool_bulletEnemyBossB);
    }
    public GameObject MakeObj(PoolType poolType)
    {
        var objList = GetPoolList(poolType);

        foreach(var item in objList)
        {
            if (item.activeSelf == false)
            {
                item.SetActive(true);
                return item;
            }
        }

        var newObject = Instantiate(GetPoolBasePrefab(poolType), this.gameObject.transform);
        objList.Add(newObject);
        return newObject;
    }

    void ObjectAllHide( List<GameObject> objList)
    { 
        foreach( var item in objList )
        {
            item.SetActive(false);
        }
    }

    public List<GameObject> GetPoolList(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.enemyL:   return pool_enemyL;
            case PoolType.enemyM:   return pool_enemyM;
            case PoolType.enemyS: return pool_enemyS;
            case PoolType.enemyBoss: return pool_enemyBoss;
            case PoolType.follower: return pool_follower;

            case PoolType.itemCoin: return pool_itemCoin;
            case PoolType.itemPower: return pool_itemPower;
            case PoolType.itemBomb: return pool_itemBomb;

            case PoolType.bulletPlayerA: return pool_bulletPlayerA;
            case PoolType.bulletPlayerB: return pool_bulletPlayerB;
            case PoolType.bulletEnemyA: return pool_bulletEnemyA;
            case PoolType.bulletEnemyB: return pool_bulletEnemyB;
            case PoolType.bulletFollowerA: return pool_bulletFollowerA;
            case PoolType.bulletEnemyBossA: return pool_bulletEnemyBossA;
            case PoolType.bulletEnemyBossB: return pool_bulletEnemyBossB;

            case PoolType.explosion: return pool_explosion;
        }

        return null;
    }

    GameObject GetPoolBasePrefab(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.enemyL: return base_enemyL;
            case PoolType.enemyM: return base_enemyM;
            case PoolType.enemyS: return base_enemyS;
            case PoolType.enemyBoss: return base_enemyBoss;
            case PoolType.follower: return base_follower;

            case PoolType.itemCoin:     return base_itemCoin;
            case PoolType.itemPower:    return base_itemPower;
            case PoolType.itemBomb:     return base_itemBomb;

            case PoolType.bulletPlayerA:    return base_bulletPlayerA;
            case PoolType.bulletPlayerB:    return base_bulletPlayerB;
            case PoolType.bulletEnemyA:     return base_bulletEnemyA;
            case PoolType.bulletEnemyB:     return base_bulletEnemyB;
            case PoolType.bulletFollowerA: return base_bulletFollowerA;
            case PoolType.bulletEnemyBossA: return base_bulletEnemyBossA;
            case PoolType.bulletEnemyBossB: return base_bulletEnemyBossB;

            case PoolType.explosion: return base_explosion;
        }

        return null;
    }
}
