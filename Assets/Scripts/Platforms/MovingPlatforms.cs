using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : InteractableObj
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rigid;

    [Header("Pos")]
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;

    [Header("State")]
    [SerializeField] private bool isMoving = true;
    [SerializeField] private float speed;

    private Vector2 start, end;
    private Vector2 dir;

    private void Awake()
    {
        start = startPos.position;
        end = endPos.position;

        dir = (end - start).normalized;

        transform.position = start;
    }

    private void FixedUpdate()
    {
        if (isMoving == false)
            return;

        rigid.velocity = dir * speed;

        if ((rigid.position - end).sqrMagnitude <= 0.03f)
        {
            Vector2 temp = end;
            end = start;
            start = temp;
            dir = (end - start).normalized;
        }
    }

    public override void Activate()
    {
        isMoving = true;
    }

    public override void Deactivate()
    {
        isMoving = false;
    }
}
