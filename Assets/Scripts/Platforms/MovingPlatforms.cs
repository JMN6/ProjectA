using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : InteractableObj, IResetable
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rigid;

    [Header("Pos")]
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;

    [Header("State")]
    [SerializeField] private bool isMoving = true;
    [SerializeField] private float speed;

    private bool isInit = false;

    private Vector2 initialStart, initialEnd;
    private Vector2 start, end;
    private Vector2 dir;

    private ButtonObject btnObj;

    private void Awake()
    {
        startPos.SetParent(null);
        endPos.SetParent(null);
        Init();
    }

    private void Init()
    {
        start = initialStart = startPos.position;
        end = initialEnd = endPos.position;

        dir = (end - start).normalized;

        transform.position = start;

        isInit = true;

        btnObj = GetComponentInChildren<ButtonObject>();
    }

    public void OnReset()
    {
        if(btnObj != null)
        {
            btnObj.OnReset();
            return;
        }

        if(isInit == false)
        {
            Init();
        }

        rigid.velocity = Vector2.zero;
        transform.position = initialStart;
    }

    public Rigidbody2D GetBody() { return rigid; }

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

        start = initialStart = startPos.position;
        end = initialEnd = endPos.position;

        dir = (end - start).normalized;

        rigid.velocity = Vector2.zero;
        transform.position = start;
    }
}
