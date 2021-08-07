using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{

    void Awake()
    {
        parentPos.Clear();
    }

    void Update()
    {
        Watch();
        Follow();

        Fire();
        Reload();
    }

    #region Move-Fire

    Vector3 followPos;
    public int followDelay;

    public Transform parent;
    Queue<Vector3> parentPos = new Queue<Vector3>();

    void Watch()
    {
        // #.INPUT pos
        if(parentPos.Contains(parent.position) == false )
            parentPos.Enqueue(parent.position);

        // #.Outpos pos
        if ( parentPos.Count > followDelay )
            followPos = parentPos.Dequeue();
        else if(parentPos.Count < followDelay)
            followPos = parent.position;
    }

    void Follow()
    {
        transform.position = followPos;
    }

    [SerializeField] float bulletSpeed = 5.0f;
    [SerializeField] float maxShotDelay = 2.0f;
    [SerializeField] float curShotDelay = 0.0f;

    void Fire()
    {
        if (curShotDelay < maxShotDelay)
            return;

        var bullet = ObjectManager.Inst.MakeObj(PoolType.bulletFollowerA).GetComponent<Rigidbody2D>();
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;

        bullet.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);

        curShotDelay = 0.0f;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }
    #endregion

}
