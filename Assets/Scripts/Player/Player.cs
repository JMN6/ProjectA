using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity, IDamagalbe
{
    [SerializeField] private float jumpPower;

    private Rigidbody2D rigid;
    private SpriteRenderer sprite;
    private Animator animator;
    private float movementDirection;
    private int curJumpCount;
    private Rigidbody2D vehicleRB;
    [SerializeField] private int maxJumpCount = 1;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip deathSFX;

    private AudioSource audioSource;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void OnMove(InputValue input)
    {
        movementDirection = input.Get<float>();

        if(movementDirection == 0)
        {
            audioSource.Stop();
        }
        else
        {
            audioSource.Play();

            if (movementDirection > 0 && sprite.flipX) sprite.flipX = false;
            else if (movementDirection < 0 && !sprite.flipX) sprite.flipX = true;
        }
    }

    public void OnJump()
    {
        if(curJumpCount < maxJumpCount)
        {
            ++curJumpCount;

            animator.SetTrigger("Jump");
            SoundManager.Instance.PlayOneShot(jumpSFX);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    private void Update()
    {
        animator.SetBool("IsWalking", movementDirection != 0);
        
        if (vehicleRB)
        {
            rigid.velocity = new Vector2(movementDirection * speed + vehicleRB.velocity.x, rigid.velocity.y);
        }
        else
        {
            rigid.velocity = new Vector2(movementDirection * speed, rigid.velocity.y);
        }


        if (rigid.velocity.y < 0.01f)
        {
            RaycastHit2D hit = Physics2D.Raycast(rigid.position, Vector2.down, 0.1f, 1 << LayerMask.NameToLayer("Ground")); // ¶¥ ·¹ÀÌ¾î¸¸

            if (hit)
            {
                curJumpCount = 0;
                animator.SetBool("IsFalling", false);

                if (!vehicleRB)
                {
                    if (hit.collider.TryGetComponent(out Rigidbody2D rb)) {
                        vehicleRB = rb;
                    }
                }
            }
            else
            {
                animator.SetBool("IsFalling", true);

                vehicleRB = null;
            }
        }
    }

    public void GetDamaged(int damage)
    {
        animator.SetBool("IsDead", true);

        SoundManager.Instance.PlayOneShot(deathSFX);
    }
}
