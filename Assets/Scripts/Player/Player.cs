using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Game.Input;


public class Player : MonoBehaviour
{
    public class PlayerAnimator
    {
        [Flags]
        public enum AnimState
        {
            Idle = 1 << 0,
            Run = 1 << 1,
            Parry = 1 << 2,
            Kick = 1 << 3,
            Jab = 1 << 4,
            Cross = 1 << 5,
            Jump = 1 << 6,
            Fall = 1 << 7,
            Death = 1 << 8,
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

    public PlayerAnimator Animator { get; private set; }
    [SerializeField] Collider2D _attackCollider;

    private IInteractable trigger;

    [Header("Attack")]
    [SerializeField] private float _comboAttackAllowTime = 0.5f;
    private float _comboAttackTimer;

    [Header("Parry")]
    [SerializeField] private float _parryingAllowTime = 0.5f;
    private float _parryingTimer;

    [Header("Hurt")]
    [SerializeField] private float invincibleDuration = 0.3f;
    private float invincibleTimer;
    private bool _isDead;

    private void Awake()
    {
        Animator = new(GetComponent<Animator>());

        ResetPlayer(transform.position, false);
    }

    private void Start()
    {
        SetupInput();
    }

    private void Update()
    {
        CountTimers();
    }

    private void SetupInput()
    {
        var input = InputManager.Instance;

        input.AddListener(Actions.Interact, TriggerFlag.Started, OnInteract);
        input.AddListener(Actions.Parry, TriggerFlag.Started, OnParry);
        input.AddListener(Actions.Attack, TriggerFlag.Started, OnAttack);
    }

    public void ResetPlayer(Vector3 _position, bool isLeft = true)
    {
        _isDead = false;

        transform.position = _position;

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

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        if (_comboAttackTimer <= 0)
        {
            InputManager.Instance.SetInputEnabled(Maps.Player, false);
            Animator.Play(PlayerAnimator.AnimState.Jab);
            _comboAttackTimer = _comboAttackAllowTime;
            Invoke(nameof(OnAttackEnd), 0.33f);
        }
        else
        {
            InputManager.Instance.SetInputEnabled(Maps.Player, false);
            CancelInvoke(nameof(OnAttackEnd));
            Animator.Play(PlayerAnimator.AnimState.Cross);
            _comboAttackTimer = 0f;
            Invoke(nameof(OnAttackEnd), 0.33f);
        }
    }

    private void OnAttackEnd()
    {
        InputManager.Instance.SetInputEnabled(Maps.Player, true);
        Animator.Stop(PlayerAnimator.AnimState.Jab);
        Animator.Stop(PlayerAnimator.AnimState.Cross);
    }

    public void OnParry(InputAction.CallbackContext ctx)
    {
        if (_parryingTimer > 0) return;

        InputManager.Instance.SetInputEnabled(Maps.Player, false);

        Animator.Play(PlayerAnimator.AnimState.Parry);
        _parryingTimer = _parryingAllowTime;

        Invoke(nameof(OnParryEnd), _parryingAllowTime);
    }

    private void OnParryEnd()
    {
        Animator.Stop(PlayerAnimator.AnimState.Parry);
        Animator.Stop(PlayerAnimator.AnimState.Kick);

        InputManager.Instance.SetInputEnabled(Maps.Player, true);

        Time.timeScale = 1;
    }

    

    public bool TryAttack()
    {
        if (invincibleTimer > 0) return true;
        if (_isDead) return true;

        if (_parryingTimer > 0)
        {
            _parryingTimer = 0f;
            Animator.Play(PlayerAnimator.AnimState.Kick);
            SoundManager.Instance.PlaySFX(Path.PlayerSFX.Parry, true);
            invincibleTimer = invincibleDuration;

            Time.timeScale = 0.5f;
            CancelInvoke(nameof(OnParryEnd));
            Invoke(nameof(OnParryEnd), 0.5f);
            return false;
        }
        else
        {
            _isDead = true;

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

    private void CountTimers()
    {
        invincibleTimer -= Time.deltaTime;
        _parryingTimer -= Time.deltaTime;
        _comboAttackTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDamagalbe target))
        {
            target.GetDamaged(1);
        }
    }
}
