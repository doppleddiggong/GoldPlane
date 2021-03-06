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
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Move();

        Fire();
        Bomb();
        Reload();
    }


    public bool isRespawnTime = false;

    void OnEnable()
    {
        Unbeatable();
        Invoke(nameof(Unbeatable), 0.5f);
    }

    public void Unbeatable()
    {
        isRespawnTime = !isRespawnTime;
        if( isRespawnTime )
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.25f);
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }

    public void EndGame()
    {
        this.gameObject.SetActive(false);

        foreach (var follower in followers)
        {
            follower.gameObject.SetActive(false);
        }
        followers.Clear();
    }

    public bool immortal = false;

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
            if (isRespawnTime == false && immortal == false )
            {
                GameManager.Inst.PlayerHit();
            }

            collision.gameObject.SetActive(false);
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
                    {
                        power++;
                        AddFollower();
                    }
                    else
                        GameManager.Inst.score += 500;
                    break;
                case Item.ItemType.Bomb:
                    GameManager.Inst.AddBomb(1);
                    break;
            }

            item.gameObject.SetActive(false);
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
        // ?????? ????
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (isControl)
        {
            if (joyControl[0]) { h = -1; v = 1; }
            if (joyControl[1]) { h = 0; v = 1; }
            if (joyControl[2]) { h = 1; v = 1; }
            if (joyControl[3]) { h = -1; v = 0; }
            if (joyControl[4]) { h = 0; v = 0; }
            if (joyControl[5]) { h = 1; v = 0; }
            if (joyControl[6]) { h = -1; v = -1; }
            if (joyControl[7]) { h = 0; v = -1; }
            if (joyControl[8]) { h = 1; v = -1; }
        }

        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1))
            h = 0;

        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1))
            v = 0;

        // ???? ????
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;
        this.transform.position += nextPos;

        // ?????????? ????
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
    uint MAX_POWER = 6;

    [SerializeField] public float speed = 1.0f;

    [SerializeField] bool autoFire = false;


    void Fire()
    {
        bool fire = false;
        if (isButtonA || autoFire )
        {
            fire = true;
        }

        if (fire == false &&
            Input.GetButtonDown("Fire1") == false )
            return;

        if (curShotDelay < maxShotDelay)
            return;

        switch(power)
        {
            case 1:
                {
                    var bullet = ObjectManager.Inst.MakeObj(PoolType.bulletPlayerA).GetComponent<Rigidbody2D>();
                    bullet.transform.position = transform.position;
                    bullet.transform.rotation = transform.rotation;

                    bullet.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);
                }

                break;


            case 2:
                {
                    var bulletL = ObjectManager.Inst.MakeObj(PoolType.bulletPlayerA).GetComponent<Rigidbody2D>();
                    bulletL.transform.position = transform.position + Vector3.left * 0.1f;
                    bulletL.transform.rotation = transform.rotation;

                    var bulletR = ObjectManager.Inst.MakeObj(PoolType.bulletPlayerA).GetComponent<Rigidbody2D>();
                    bulletR.transform.position = transform.position + Vector3.right * 0.1f;
                    bulletR.transform.rotation = transform.rotation;

                    bulletL.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);
                    bulletR.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);
                }
                break;

            case 3:
            default:
                {
                    var bulletL = ObjectManager.Inst.MakeObj(PoolType.bulletPlayerA).GetComponent<Rigidbody2D>();
                    bulletL.transform.position = transform.position + Vector3.left * 0.3f;
                    bulletL.transform.rotation = transform.rotation;

                    var bulletC = ObjectManager.Inst.MakeObj(PoolType.bulletPlayerB).GetComponent<Rigidbody2D>(); 
                    bulletC.transform.position = transform.position;
                    bulletC.transform.rotation = transform.rotation;

                    var bulletR = ObjectManager.Inst.MakeObj(PoolType.bulletPlayerA).GetComponent<Rigidbody2D>();
                    bulletR.transform.position = transform.position + Vector3.right * 0.3f;
                    bulletR.transform.rotation = transform.rotation;

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
        bool fire = false;
        
        if( isButtonB )
        {
            fire = true;
        }
        if (Input.GetButtonDown("Fire2"))
        {
            fire = true;
        }

        if (fire == false )
            return;

        GameManager.Inst.ExecuteBomb();
    }


    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }
    #endregion

    #region Follower

    List<Follower> followers = new List<Follower>();

    void AddFollower()
    {
        if (power >= 4)
        {
            var follower = ObjectManager.Inst.MakeObj(PoolType.follower).GetComponent<Follower>();
            follower.transform.position = this.gameObject.transform.position;
            followers.Add(follower);

            if (followers.Count ==  1)
            {
                follower.parent = this.gameObject.transform;
            }
            else
            {
                follower.parent = followers[followers.Count - 2].transform;
            }
        }
    }

    public void FollowerActivate( bool state )
    {
        foreach( var follower in followers )
        {
            //follower.gameObject.SetActive(state);

            if (state)
            {
                follower.transform.position = this.gameObject.transform.position;
                follower.GetComponent<SpriteRenderer>().color = Color.white;
            }
            else
            {
                follower.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.25f);
            }
        }
    }
    #endregion




    #region joyPad
    bool[] joyControl = new bool[9];
    bool isControl;
    public void JoyPanel(int type)
    {
        for( int i = 0; i < 9; i++ )
        {
            joyControl[i] = i == type;
        }
    }
    public void joyDown()
    {
        isControl = true;
    }
    public void joyUp()
    {
        isControl = false;
    }

    bool isButtonA;
    bool isButtonB;

    public void ButtonADown()
    {
        isButtonA = true;
    }

    public void ButtonAUp()
    {
        isButtonA = false;
    }

    public void ButtonBDown()
    {
        isButtonB = true;
    }
    #endregion
}
