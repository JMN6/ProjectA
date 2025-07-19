using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    [SerializeField] private float jumpPower;

    private Rigidbody2D rigid;
    private SpriteRenderer sprite;
    private Animator animator;
    private float movementDirection;
    private int curJumpCount;
    private Rigidbody2D vehicleRB;
    [SerializeField] private int maxJumpCount = 1;
    [SerializeField] private float validParryingTime = 1f;
    private bool isParrying;
    private Coroutine parryCoroutine;

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

    public void OnInteract()
    {

    }

    public void OnParry()
    {
        if(!isParrying)
        {
            parryCoroutine = StartCoroutine(CoPrepareParrying());
        }
    }

    public IEnumerator CoPrepareParrying()
    {
        animator.SetBool("IsParrying", true);
        isParrying = true;

        float timer = 0f;

        while (timer < validParryingTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // ½ÇÆÐ
        animator.SetBool("IsParrying", false);
        isParrying = false;
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


        if (rigid.velocity.y < 0.051f)
        {
            var hit = Physics2D.OverlapCircle(transform.position, 0.5f, 1 << LayerMask.NameToLayer("Ground"));

            if (hit)
            {
                curJumpCount = 0;
                animator.SetBool("IsFalling", false);

                if (!vehicleRB)
                {
                    if (hit.TryGetComponent(out Rigidbody2D rb)) {
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

    public bool TryAttack()
    {
        if (isParrying)
        {
            animator.SetTrigger("Parry");

            animator.SetBool("IsParrying", false);

            StopCoroutine(parryCoroutine);
            //speed = 5;
            SoundManager.Instance.PlaySFX(jumpSFX);
            isParrying = false;

            return false;
        }
        else
        {
            animator.SetBool("IsDead", true);

            SoundManager.Instance.PlayOneShot(deathSFX);

            return true;
        }
    }
}
