using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float speed;

    public int startIndex;
    public int endIndex;

    public Transform[] sprites;

    float viewHeight;

    void Awake()
    {
        viewHeight = Camera.main.orthographicSize *2.0f;
    }

    private void Update()
    {
        Move();
        Scrolling();
    }

    private void Move()
    {
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;
        transform.position += nextPos;
    }

    void Scrolling()
    {
        if (sprites[endIndex].position.y < viewHeight * (-1.0f))
        {
            // #.Sprite ReUse
            var backSpritesPos = sprites[startIndex].transform.localPosition;

            sprites[endIndex].transform.localPosition = backSpritesPos + Vector3.up * viewHeight;

            // #.Cursor Index Change
            int startIndexSave = startIndex;
            startIndex = endIndex;
            endIndex = (startIndexSave - 1 == -1) ? sprites.Length - 1 : startIndexSave - 1;
        }
    }
}
