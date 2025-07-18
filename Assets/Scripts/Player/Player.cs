using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float jumpPower;

    private Rigidbody2D rigid;
    private int moveDirect;
    private int curJumpCount;
    private int maxJumpCount;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputValue input)
    {
        float val = input.Get<float>();

        if (val < 0) moveDirect = -1;
        else moveDirect = 1;
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
        rigid.velocity = new Vector2(moveDirect, rigid.velocity.y);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // ¶¥ÀÌ¸é
        if (true)
        {
            curJumpCount = 0;
        }
    }
}
