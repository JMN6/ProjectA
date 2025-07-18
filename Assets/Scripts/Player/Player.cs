using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity, IDamagalbe
{
    [SerializeField] private float jumpPower;

    private Rigidbody2D rigid;
    private SpriteRenderer sprite;
    private Animator animator;
    private float moveDirect;
    private int curJumpCount;
    [SerializeField] private int maxJumpCount = 1;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void OnMove(InputValue input)
    {
        moveDirect = input.Get<float>();

        if (moveDirect > 0 && sprite.flipX) sprite.flipX = false;
        else if (moveDirect < 0 && !sprite.flipX) sprite.flipX = true;
    }

    public void OnJump()
    {
        if(curJumpCount < maxJumpCount)
        {
            ++curJumpCount;

            animator.SetTrigger("Jump");
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    private void Update()
    {
        Debug.Log(curJumpCount);
        rigid.velocity = new Vector2(moveDirect * speed, rigid.velocity.y);
        animator.SetBool("IsWalking", moveDirect != 0);

        if (rigid.velocity.y < 0.01f)
        {
            RaycastHit2D hit = Physics2D.Raycast(rigid.position, Vector2.down, 0.1f, 1 << LayerMask.NameToLayer("Ground")); // ¶¥ ·¹ÀÌ¾î¸¸

            if (hit)
            {
                curJumpCount = 0;
                animator.SetBool("IsFalling", false);
            }
            else
            {
                animator.SetBool("IsFalling", true);
            }
        }
    }

    public void GetDamaged(int damage)
    {
    }
}
