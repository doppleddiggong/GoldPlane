using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 1.0f;
    
    bool isTouchTop;
    bool isTouchBottom;
    bool isTouchRight;
    bool isTouchLeft;

    [SerializeField] Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        // 입력정보
        float h = Input.GetAxisRaw("Horizontal");

        if( (isTouchRight && h == 1) ||
            (isTouchLeft && h == -1) )
            h = 0;

        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && v == 1) ||
            (isTouchBottom && v == -1))
            v = 0;

        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;
        this.transform.position += nextPos;

        if (Input.GetButtonDown("Horizontal") ||
            Input.GetButtonUp("Horizontal"))
        {
            anim.SetInteger("Input", (int)h);
        }

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
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
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
}
