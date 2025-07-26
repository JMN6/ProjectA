using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Game.Input;

public class Player : Entity
{
    public class PlayerAnimator
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
                stateToHash.TryAdd(state, UnityEngine.Animator.StringToHash(state.ToString()));
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

        public void Stop(AnimState state, float time = 0f)
        {
            stateFlag &= ~(int)state;

            if (stateFlag > (int)state) return;

            int curFlag = 1;

            while ((curFlag << 1) <= stateFlag)
                curFlag <<= 1;

            if (stateToHash.TryGetValue((AnimState)curFlag, out int hash))
            {
                if (time == 0f) animator.Play(hash);
                else animator.CrossFade(hash, time);
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

    private SpriteRenderer sprite;
    public PlayerAnimator Animator { get; private set; }

    private Rigidbody2D vehicleRB;
    private IInteractable trigger;

    [Header("Parry")]
    private bool isParrying;
    [SerializeField] private float validParryingTime = 0.5f;
    private Coroutine parryCoroutine;

    [Header("Hurt")]
    private float invincibleTimer;
    [SerializeField] private float invincibleDuration = 0.3f;


    private CameraFunctions cameraFunctions;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        Animator = new(GetComponent<Animator>());

        ResetPlayer(transform.position, false);
    }

    private void Start()
    {
        cameraFunctions = Camera.main.GetComponent<CameraFunctions>();
        SetupInput();
    }

    private void Update()
    {
        CheckInvincible();
    }

    private void SetupInput()
    {
        var input = InputManager.Instance;

        input.AddListener(Actions.Interact, TriggerFlag.Started, OnInteract);
        input.AddListener(Actions.Parry, TriggerFlag.Started, OnParry);
    }

    public void ResetPlayer(Vector3 _position, bool isLeft = true)
    {
        currentHealth = maxHealth;

        sprite.flipX = isLeft;
        transform.position = _position;

        vehicleRB = null;
        Animator.Clear();
    }

    

    

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (trigger == null)
        {
            return;
        }

        trigger.Interact();
        trigger = null;
    }

    public void OnParry(InputAction.CallbackContext ctx)
    {
        if (!isParrying)
        {
            InputManager.Instance.SetInputEnabled(Maps.Player, false);

            Animator.Play(PlayerAnimator.AnimState.Parry);
            isParrying = true;

            Invoke(nameof(OnParryEnd), validParryingTime);
        }
    }

    private void OnParryEnd()
    {
        isParrying = false;

        Animator.Stop(PlayerAnimator.AnimState.Parry);
        Animator.Stop(PlayerAnimator.AnimState.Kick);

        InputManager.Instance.SetInputEnabled(Maps.Player, true);

        Time.timeScale = 1;
    }

    

    public bool TryAttack()
    {
        if (invincibleTimer > 0) return true;
        if (currentHealth <= 0) return true;

        if (isParrying)
        {
            isParrying = false;
            Animator.Play(PlayerAnimator.AnimState.Kick);
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
            currentHealth = 0;

            GameManager.Instance.GameOver();

            Animator.Play(PlayerAnimator.AnimState.Death, 0.25f);

            SoundManager.Instance.PlaySFX(Path.PlayerSFX.Dead);

            return true;
        }
    }

    public void CanInteract(bool _val, IInteractable _trigger = null)
    {
        trigger = _trigger;
    }

    private void CheckInvincible()
    {
        if (invincibleTimer > 0) invincibleTimer -= Time.deltaTime;
    }

    
}
