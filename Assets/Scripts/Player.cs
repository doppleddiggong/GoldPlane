using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    bool isTouchTop;
    bool isTouchBottom;
    bool isTouchRight;
    bool isTouchLeft;

    [SerializeField] Animator anim;

    [SerializeField] GameObject bulletObjA;
    [SerializeField] GameObject bulletObjB;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        Move();

        Fire();
        Bomb();
        Reload();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        { 
            switch(collision.gameObject.name)
            {
                case "Top":     isTouchTop = true;      break;
                case "Bottom":  isTouchBottom = true;   break;
                case "Right":   isTouchRight = true;    break;
                case "Left":    isTouchLeft = true;     break;
            }
        }
        else if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("EnemyBullet"))
        {
            GameManager.Inst.PlayerHit();
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.CompareTag("Item"))
        {
            var item = collision.gameObject.GetComponent<Item>();
            switch( item.itemType )
            {
                case Item.ItemType.Coin:
                    GameManager.Inst.score += 1000;
                    break;
                case Item.ItemType.Power:
                    if (power != MAX_POWER)
                        power++;
                    else
                        GameManager.Inst.score += 500;
                    break;
                case Item.ItemType.Bomb:
                    GameManager.Inst.AddBomb(1);
                    break;
            }

            Destroy(collision.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Border"))
        {
            switch (collision.gameObject.name)
            {
                case "Top": isTouchTop = false; break;
                case "Bottom": isTouchBottom = false; break;
                case "Right": isTouchRight = false; break;
                case "Left": isTouchLeft = false; break;
            }
        }

    }

    #region Move-Fire

    void Move()
    {
        // 경계선 체크
        float h = Input.GetAxisRaw("Horizontal");
        if ((isTouchRight && h == 1) ||
            (isTouchLeft && h == -1))
            h = 0;

        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && v == 1) ||
            (isTouchBottom && v == -1))
            v = 0;

        // 이동 제어
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;
        this.transform.position += nextPos;

        // 애니메이션 제어
        if (Input.GetButtonDown("Horizontal") ||
            Input.GetButtonUp("Horizontal"))
        {
            anim.SetInteger("Input", (int)h);
        }
    }

    [SerializeField] float bulletSpeed = 10.0f;
    [SerializeField] float maxShotDelay = 0.2f;
    [SerializeField] float curShotDelay = 0.0f;

    [SerializeField] public uint power;
    uint MAX_POWER = 3;

    [SerializeField] public float speed = 1.0f;

    [SerializeField] bool autoFire = false;


    void Fire()
    {
        if ( autoFire == false &&
            Input.GetButtonDown("Fire1") == false )
            return;

        if (curShotDelay < maxShotDelay)
            return;

        switch(power)
        {
            case 1:
                {
                    var bullet = Instantiate(bulletObjA, transform.position, transform.rotation).GetComponent<Rigidbody2D>();
                    bullet.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);
                }

                break;


            case 2:
                {
                    var bulletR = Instantiate(bulletObjA, transform.position + Vector3.right * 0.1f, transform.rotation).GetComponent<Rigidbody2D>();
                    var bulletL = Instantiate(bulletObjA, transform.position + Vector3.left * 0.1f, transform.rotation).GetComponent<Rigidbody2D>();

                    bulletR.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);
                    bulletL.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);
                }
                break;

            case 3:
                {
                    var bulletR = Instantiate(bulletObjA, transform.position + Vector3.right * 0.3f, transform.rotation).GetComponent<Rigidbody2D>();
                    var bulletL = Instantiate(bulletObjA, transform.position + Vector3.left * 0.3f, transform.rotation).GetComponent<Rigidbody2D>();
                    var bulletC = Instantiate(bulletObjB, transform.position, transform.rotation).GetComponent<Rigidbody2D>();

                    bulletR.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);
                    bulletL.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);
                    bulletC.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);
                }

                break;
        }


        curShotDelay = 0.0f;
    }

    void Bomb()
    {
        if( Input.GetButtonDown("Fire2") == false )
            return;

        GameManager.Inst.ExecuteBomb();
    }


    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }
    #endregion
}
