using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Sprite[] sprites;

    public float speed;
    public int MaxHealth;
    private int health;
    public int enemyScore;

    SpriteRenderer spriteRenderer;
    Animator anim;

    void Awake()
    {
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
    
        if(enemyType == EnemyType.BOSS)
            anim = this.gameObject.GetComponent<Animator>();
    }

    void OnEnable()
    {
        health = MaxHealth;

        if (enemyType == EnemyType.BOSS)
        {
            Invoke(nameof(Stop), 2.0f);
        }
    }

    void Update()
    {
        if (enemyType == EnemyType.BOSS)
            return;

        Fire();
        Reload();
    }

    public void OnHit(int dmg)
    {
        if (health <= 0)
            return;

        health -= dmg;
        if (enemyType == EnemyType.BOSS)
        {
            anim.SetTrigger("OnHit");
        }
        else
        {
            spriteRenderer.sprite = sprites[1];
            Invoke("ReturnSprite", 0.1f);
        }

        if ( health <= 0 )
        {
            GameManager.Inst.score += enemyScore;

            // Random Ratio Item Drop
            int rand = Random.Range(0, 10);
            if (enemyType == EnemyType.BOSS)
                rand = 0;

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
            CancelInvoke();
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if( collision.gameObject.CompareTag("BorderBullet") && enemyType != EnemyType.BOSS )
        {
            this.gameObject.SetActive(false);
        }
        else if(collision.gameObject.CompareTag("PlayerBullet"))
        {
            var bullet = collision.gameObject.GetComponent<Bullet>();
            this.OnHit(bullet.dmg);

            collision.gameObject.SetActive(false);
        }
    }

    [SerializeField] float maxShotDelay = 0.2f;
    [SerializeField] float curShotDelay = 0.0f;

    enum EnemyType{ S, M, L, BOSS }
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
        else if (enemyType == EnemyType.L)
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

    void Stop()
    {
        if (gameObject.activeSelf == false)
            return;

        var rigid = gameObject.GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        Invoke(nameof(Think), 2.0f);
    }

    int patternIndex = -1;
    int curPatternCount;
    int[] maxPatternCount = { 2, 3, 99, 10 };

    void Think()
    {
        if (health <= 0)
            return;

        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;
        curPatternCount = 0;
        // 패턴 테스트
        patternIndex = 3;

        switch (patternIndex)
        {
            case 0:
                FireForward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
        }
    }
    void FireForward()
    {
        // 앞으로 4발 발사
        curPatternCount++;

        for (int i = 0; i < 4; i++)
        {
            var rigid = ObjectManager.Inst.MakeObj(PoolType.bulletEnemyBossA).GetComponent<Rigidbody2D>();

            switch( i )
            {
                case 0:
                    rigid.transform.position = transform.position + Vector3.right * 0.3f;
                    rigid.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
                    break;
                case 1:
                    rigid.transform.position = transform.position + Vector3.right * 0.45f;
                    rigid.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
                    break;
                case 2:
                    rigid.transform.position = transform.position + Vector3.left * 0.3f;
                    rigid.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
                    break;
                case 3:
                    rigid.transform.position = transform.position + Vector3.left * 0.45f;
                    rigid.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
                    break;
            }
        }

        if ( curPatternCount < maxPatternCount[patternIndex])
            Invoke(nameof(FireForward), 2.0f);
        else
            Invoke(nameof(Think), 3.0f);
    }

    void FireShot()
    {
        // 플레이어 방향 샷건
        curPatternCount++;

        for (int i = 0; i < 5; i++)
        {
            var rigid = ObjectManager.Inst.MakeObj(PoolType.bulletEnemyBossB).GetComponent<Rigidbody2D>();

            Vector2 dirVec = GameManager.Inst.player.transform.position - transform.position;
            dirVec += new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0.0f, 2.0f));

            rigid.transform.position = transform.position + Vector3.right * 0.3f;
            rigid.AddForce(dirVec.normalized *3 , ForceMode2D.Impulse);
        }

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke(nameof(FireShot), 3.5f);
        else
            Invoke(nameof(Think), 3.0f);
    }

    void FireArc()
    {
        // 부채모양으로 발사
        var rigid = ObjectManager.Inst.MakeObj(PoolType.bulletEnemyA).GetComponent<Rigidbody2D>();
        rigid.transform.position = transform.position;

        Vector2 dirVec = new Vector2( Mathf.Cos(Mathf.PI * 9.0f * (float)curPatternCount/ maxPatternCount[patternIndex]), -1);
        rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);

        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke(nameof(FireArc), 0.15f);
        else
            Invoke(nameof(Think), 3.0f);
    }
    void FireAround()
    {
        // 원형태로 전체 공격
        int roundNumA = 50;
        int roundNumB = 40;
        int roundNum = curPatternCount % 2 == 0? roundNumA : roundNumB;

        for ( int i = 0; i < roundNum; i++)
        {
            var rigid = ObjectManager.Inst.MakeObj(PoolType.bulletEnemyBossA).GetComponent<Rigidbody2D>();
            rigid.transform.position = transform.position;

            Vector2 dirVec = new Vector2( Mathf.Cos(Mathf.PI * 2.0f * (float)i / roundNum),
                                          Mathf.Sin(Mathf.PI * 2.0f * (float)i / roundNum) );
            rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

            Vector3 rotVec = Vector3.forward * 360 * i / roundNum + Vector3.forward * 90.0f;
            rigid.transform.Rotate(rotVec);
        }

        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke(nameof(FireAround), 0.7f);
        else
            Invoke(nameof(Think), 3.0f);
    }
}
