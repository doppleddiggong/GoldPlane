using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Sprite[] sprites;

    public float speed;
    public int health;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;

    public void Awake()
    {
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();

        rigid = this.gameObject.GetComponent<Rigidbody2D>();
        rigid.velocity = Vector3.down * speed;
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
        if( collision.gameObject.tag == "BorderBullet")
        {
            Destroy(this.gameObject);
        }
        else if(collision.gameObject.tag == "PlayerBullet")
        {
            var bullet = collision.gameObject.GetComponent<Bullet>();
            this.OnHit(bullet.dmg);


            Destroy(collision.gameObject);
        }
    }
}
