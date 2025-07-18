using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour, IActivatable
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
        rigid.MovePosition(start);

        speed *= 0.05f;
    }

    private void FixedUpdate()
    {
        if (isMoving == false)
            return;

        Vector2 newPosition = rigid.position + dir * speed;
        if ((newPosition - end).sqrMagnitude <= 0.03f)
        {
            newPosition = end;
            Vector2 temp = end;
            end = start;
            start = temp;
            dir = (end - start).normalized;
        }

        rigid.MovePosition(newPosition);
    }

    public void Activate()
    {
        isMoving = true;
    }

    public void Deactivate()
    {
        isMoving = false;
    }
}
