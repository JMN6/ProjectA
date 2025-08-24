using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rigid;
    public Rigidbody2D Rigid => rigid;

    public void SetVelocity(float x, float y)
    {
        SetVelocity(new Vector2 (x, y));
    }

    public virtual void SetVelocity(Vector2 dir)
    {
        rigid.linearVelocity = dir;
    }

    public virtual void MovePosition(Vector2 pos)
    {
        rigid.MovePosition(pos);
    }
}
