using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Sprite[] sprites;

    public float speed;
    public int health;

    SpriteRenderer spriteRenderer;

    [SerializeField] GameObject bulletObjA;
    [SerializeField] GameObject bulletObjB;

    public void Awake()
    {
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Fire();
        Reload();
    }

    void OnHit(int dmg)
    {
        health -= dmg;  
        spriteRenderer.sprite = sprites[1];

        Invoke("ReturnSprite", 0.1f);

        if ( health <= 0 )
        {
            Destroy(this.gameObject);
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if( collision.gameObject.CompareTag("BorderBullet"))
        {
            Destroy(this.gameObject);
        }
        else if(collision.gameObject.CompareTag("PlayerBullet"))
        {
            var bullet = collision.gameObject.GetComponent<Bullet>();
            this.OnHit(bullet.dmg);


            Destroy(collision.gameObject);
        }
    }

    [SerializeField] float maxShotDelay = 0.2f;
    [SerializeField] float curShotDelay = 0.0f;

    enum EnemyType{ S, M, L }
    [SerializeField] EnemyType enemyType = EnemyType.S;

    void Fire()
    {
        if (curShotDelay < maxShotDelay)
            return;

        if (enemyType == EnemyType.S)
        {
            var bullet = Instantiate(bulletObjA, transform.position, transform.rotation).GetComponent<Rigidbody2D>();
            Vector3 dirVec = GameManager.Inst.player.transform.position - transform.position;
            bullet.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }
        if (enemyType == EnemyType.L)
        {
            var bulletL = Instantiate(bulletObjB, transform.position + Vector3.left * 0.3f, transform.rotation).GetComponent<Rigidbody2D>();
            var bulletR = Instantiate(bulletObjB, transform.position + Vector3.right * 0.3f, transform.rotation).GetComponent<Rigidbody2D>();

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
