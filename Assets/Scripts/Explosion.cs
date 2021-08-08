using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void StartExplosion(PoolType poolType)
    {
        anim.SetTrigger("OnExplosion");

        switch (poolType)
        {
            case PoolType.enemyS:
                transform.localScale = Vector3.one * 0.7f;
                break;

            case PoolType.enemyL:
                transform.localScale = Vector3.one * 2.0f;
                break;

            case PoolType.enemyBoss:
                transform.localScale = Vector3.one * 3.0f;
                break;

            case PoolType.enemyM:
            default:
                transform.localScale = Vector3.one * 1.0f;
                break;
        }
    }

    public void EndExplosion()
    {
        this.gameObject.SetActive(false);
    }
}
