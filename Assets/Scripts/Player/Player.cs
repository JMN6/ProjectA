using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    private class PlayerAnimator
    {
        [Flags]
        public enum AnimState
        {
            Idle = 1 << 0,
            Run = 1 << 1,
            Jump = 1 << 2,
            Fall = 1 << 3,
            Parry = 1 << 4,
            Kick = 1 << 5,
            Death = 1 << 6,
        }

        private Animator animator;

        private Dictionary<AnimState, int> stateToHash;
        private const AnimState defaultState = AnimState.Idle;
        private int stateFlag;

        public PlayerAnimator(Animator anim)
        {
            animator = anim;

            stateToHash = new();

            foreach (AnimState state in Enum.GetValues(typeof(AnimState)))
            {
                stateToHash.TryAdd(state, Animator.StringToHash(state.ToString()));
            }
        }

        public void Play(AnimState state, float time = 0f)
        {
            int curState = stateFlag;
            stateFlag |= (int)state;
            if (curState > (int)state) return;

            if (stateToHash.TryGetValue(state, out int hash))
            {
                if (time == 0f) animator.Play(hash);
                else animator.CrossFade(hash, time);
            }
        }

        public void Stop(AnimState state)
        {
            stateFlag &= ~(int)state;

            if (stateFlag > (int)state) return;

            int curFlag = 1;

            while ((curFlag << 1) <= stateFlag)
                curFlag <<= 1;

            if (stateToHash.TryGetValue((AnimState)curFlag, out int hash))
            {
                animator.Play(hash);
            }
        }

        public void Clear()
        {
            stateFlag = (int)defaultState;

            if (stateToHash.TryGetValue(defaultState, out int hash))
            {
                animator.Play(hash);
            }
        }
    }

    [SerializeField] private float jumpPower;

    private Rigidbody2D rigid;
    private SpriteRenderer sprite;
    private PlayerAnimator animator;
    private float movementDirection;
    private int curJumpCount;
    private Rigidbody2D vehicleRB;
    [SerializeField] private int maxJumpCount = 1;
    [SerializeField] private float validParryingTime = 0.3f;
    private bool isParrying;
    private Coroutine parryCoroutine;


    private bool isInteractable = false;
    private IInteractable trigger;

    private bool isDied = false;
    private CameraFunctions cameraFunctions;

    private float invincibleTimer = 0f;
    private float invincibleDuration = 0.3f;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = new(GetComponent<Animator>());

        ResetPlayer(transform.position, false);
    }

    private void Start()
    {
        cameraFunctions = Camera.main.GetComponent<CameraFunctions>();
    }

    public void ResetPlayer(Vector3 _position, bool isLeft = true)
    {
        isDied = false;
        isInteractable = false;

        sprite.flipX = isLeft;
        transform.position = _position;

        vehicleRB = null;
        animator.Clear();
    }

    public void OnMove(InputValue _input)
    {
        movementDirection = _input.Get<float>();

        if (movementDirection == 0)
        {
            // Anim
            animator.Stop(PlayerAnimator.AnimState.Run);

            // Sound
            SoundManager.Instance.StopSFX(Path.PlayerSFX.Walk);
        }
        else
        {
            // Sprite
            sprite.flipX = movementDirection < 0;

            // Anim
            animator.Play(PlayerAnimator.AnimState.Run, 0.1f);

            // Sound
            SoundManager.Instance.PlaySFX(Path.PlayerSFX.Walk);
        }
    }

    public void OnJump()
    {
        if (curJumpCount < maxJumpCount)
        {
            ++curJumpCount;
            vehicleRB = null;

            // remove y velocity
            rigid.velocity *= Vector2.right;
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

            // Anim
            animator.Play(PlayerAnimator.AnimState.Jump, 0.1f);

            // Sound
            SoundManager.Instance.StopSFX(Path.PlayerSFX.Walk);
            SoundManager.Instance.PlaySFX(Path.PlayerSFX.Jump, true);
        }
    }

    public void OnInteract()
    {
        if (isInteractable == false)
        {
            return;
        }

        trigger.Interact();
        isInteractable = false;
        trigger = null;
    }

    public void OnParry()
    {
        if (!isParrying)
        {
            InputManager.Instance.SetPlayerInput(false);

            animator.Play(PlayerAnimator.AnimState.Parry);
            isParrying = true;

            Invoke(nameof(OnParryEnd), validParryingTime);
        }
    }

    private void OnParryEnd()
    {
        isParrying = false;

        animator.Stop(PlayerAnimator.AnimState.Parry);
        animator.Stop(PlayerAnimator.AnimState.Kick);

        InputManager.Instance.SetPlayerInput(true);

        Time.timeScale = 1;
    }

    private void Update()
    {
        if (invincibleTimer > 0) invincibleTimer -= Time.deltaTime;

        CheckGround();

        float x = vehicleRB ? movementDirection * speed + vehicleRB.velocity.x : movementDirection * speed;
        float y = vehicleRB ? vehicleRB.velocity.y : rigid.velocity.y;

        rigid.velocity = Vector2.right * x + Vector2.up * y;
    }

    private void CheckGround()
    {
        if (rigid.velocity.y > 0.01f) return;

        Collider2D[] hits = new Collider2D[1];

        int hit = Physics2D.OverlapBoxNonAlloc(transform.position, new Vector2(0.65f, 0.01f), 0f, hits, 1 << LayerMask.NameToLayer("Ground"));
        
        if (hit > 0)
        {
            if (vehicleRB) return;

            if (hits[0].TryGetComponent(out MovingPlatforms vehicle)) {
                if (vehicle.transform.position.y < transform.position.y)
                {
                    vehicleRB = vehicle.GetBody();
                }
            }

            if (movementDirection != 0) SoundManager.Instance.PlaySFX(Path.PlayerSFX.Walk);
            animator.Stop(PlayerAnimator.AnimState.Jump);
            animator.Stop(PlayerAnimator.AnimState.Fall);

            curJumpCount = 0;
        }
        else
        {
            vehicleRB = null;

            animator.Stop(PlayerAnimator.AnimState.Jump);
            animator.Play(PlayerAnimator.AnimState.Fall);
        }
    }

    public bool TryAttack()
    {
        if (invincibleTimer > 0)
            return true;

        if (isDied == true)
            return true;

        if (isParrying)
        {
            isParrying = false;
            animator.Play(PlayerAnimator.AnimState.Kick);
            SoundManager.Instance.PlaySFX(Path.PlayerSFX.Parry, true);
            cameraFunctions?.Shake();
            invincibleTimer = invincibleDuration;


            Time.timeScale = 0.5f;
            CancelInvoke(nameof(OnParryEnd));
            Invoke(nameof(OnParryEnd), 0.5f);
            return false;
        }
        else
        {
            isDied = true;
            GameManager.Instance.GameOver();

            animator.Play(PlayerAnimator.AnimState.Death, 0.25f);

            SoundManager.Instance.PlaySFX(Path.PlayerSFX.Dead);

            return true;
        }
    }

    public void CanInteract(bool _val, IInteractable _trigger = null)
    {
        isInteractable = _val;
        trigger = _trigger;
    }
}
