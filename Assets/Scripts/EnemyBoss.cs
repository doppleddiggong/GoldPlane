using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    public Sprite[] sprites;

    public float speed;
    public int MaxHealth;
    private int health;
    public int enemyScore;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        health = MaxHealth;
    }

    void Update()
    {
        Fire();
        Reload();
    }

    public void OnHit(int dmg)
    {
        if (health <= 0)
            return;

        health -= dmg;
        spriteRenderer.sprite = sprites[1];
        Invoke("ReturnSprite", 0.1f);

        if (health <= 0)
        {
            GameManager.Inst.score += enemyScore;

            // Random Ratio Item Drop
            int rand = Random.Range(0, 10);
            if (rand < 5)
            { }
            else if (rand < 6)
            {
                // 30p
                var obj = ObjectManager.Inst.MakeObj(PoolType.itemCoin);
                obj.transform.position = transform.position;
            }
            else if (rand < 8)
            {
                // 20p
                var obj = ObjectManager.Inst.MakeObj(PoolType.itemPower);
                obj.transform.position = transform.position;
            }
            else if (rand < 10)
            {
                // 10p
                var obj = ObjectManager.Inst.MakeObj(PoolType.itemBomb);
                obj.transform.position = transform.position;
            }

            this.gameObject.SetActive(false);
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BorderBullet"))
        {
            this.gameObject.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            var bullet = collision.gameObject.GetComponent<Bullet>();
            this.OnHit(bullet.dmg);

            collision.gameObject.SetActive(false);
        }
    }

    [SerializeField] float maxShotDelay = 0.2f;
    [SerializeField] float curShotDelay = 0.0f;

    enum EnemyType { S, M, L }
    [SerializeField] EnemyType enemyType = EnemyType.S;

    void Fire()
    {
        if (curShotDelay < maxShotDelay)
            return;

        if (enemyType == EnemyType.S)
        {
            var bullet = ObjectManager.Inst.MakeObj(PoolType.bulletEnemyA).GetComponent<Rigidbody2D>();
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;

            Vector3 dirVec = GameManager.Inst.player.transform.position - transform.position;
            bullet.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }
        if (enemyType == EnemyType.L)
        {
            var bulletL = ObjectManager.Inst.MakeObj(PoolType.bulletEnemyB).GetComponent<Rigidbody2D>();
            bulletL.transform.position = transform.position + Vector3.left * 0.3f;
            bulletL.transform.rotation = transform.rotation;

            var bulletR = ObjectManager.Inst.MakeObj(PoolType.bulletEnemyB).GetComponent<Rigidbody2D>();
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;
            bulletR.transform.rotation = transform.rotation;

            Vector3 dirVecL = GameManager.Inst.player.transform.position - (transform.position + Vector3.left * 0.3f);
            Vector3 dirVecR = GameManager.Inst.player.transform.position - (transform.position + Vector3.right * 0.3f);

            bulletL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse);
            bulletR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse);
        }

        curShotDelay = 0.0f;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }
}
