using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigid;
    public Rigidbody2D Rigid => rigid;

    public void SetVelocity(float x, float y)
    {
        SetVelocity(new Vector2 (x, y));
    }

    public void SetVelocity(Vector2 dir)
    {
        rigid.velocity = dir;
    }

    public void MovePosition(Vector2 pos)
    {
        rigid.MovePosition(pos);
    }
}
