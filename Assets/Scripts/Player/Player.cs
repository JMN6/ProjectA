using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity, IDamagalbe
{
    [SerializeField] private float jumpPower;

    private Rigidbody2D rigid;
    private float moveDirect;
    private int curJumpCount;
    [SerializeField] private int maxJumpCount = 1;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputValue input)
    {
        moveDirect = input.Get<float>();
        return;
        if(input.isPressed == true)
        {
            float val = input.Get<float>();
            Debug.Log(val);
            moveDirect = Convert.ToInt32(Mathf.Sign(val));
            return;
        }
        moveDirect = 0;
    }

    public void OnJump()
    {
        if(curJumpCount < maxJumpCount)
        {
            ++curJumpCount;

            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    private void Update()
    {
        rigid.velocity = new Vector2(moveDirect * speed, rigid.velocity.y);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // ¶¥ÀÌ¸é
        if (true)
        {
            curJumpCount = 0;
        }
    }

    public void GetDamaged(int damage)
    {
    }
}
