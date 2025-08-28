using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Input;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour, ILightSensitive, IDamageable
{
    [SerializeField] PlayerStatsSO _stat;
    public int CurHP { get; private set; }
    public int MaxHP { get; private set; }

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
    [SerializeField] PlayerMovement _movement;

    public PlayerAnimator Animator { get; private set; }
    [SerializeField] SpriteRenderer _sprite;
    [SerializeField] Animator _animator;

    [SerializeField] bool _isFacingRight;

    private IInteractable _trigger;

    [Header("Parry")]
    [SerializeField] private float _parryingAllowTime = 0.5f;
    private float _parryingTimer;
    CancellationTokenSource _parryingTokenSource; 

    [Header("Hurt")]
    [SerializeField] private float _invincibleDuration = 0.3f;
    private float _invincibleTimer;
    private bool _isDead;

    bool _isInLight = false;

    public event Action<GameObject> onPlayerHitBefore;
    public event Action onPlayerHPChanged;


    private void Awake()
    {
        Animator = new(GetComponent<Animator>());

        MaxHP = _stat.DefaultMaxHP;
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
        input.AddListener(Actions.Move, TriggerFlag.Started | TriggerFlag.Canceled, OnMove);
        input.AddListener(Actions.Jump, TriggerFlag.Started | TriggerFlag.Canceled, OnJump);
    }

    public void ResetPlayer(Vector3 _position, bool isLeft = true)
    {
        CurHP = MaxHP;
        onPlayerHPChanged?.Invoke();

        _isFacingRight = true;
        _sprite.transform.rotation = Quaternion.identity;

        _isDead = false;

        transform.position = _position;

        Animator.Clear();
    }

    #region Input

    private void OnMove(InputAction.CallbackContext ctx)
    {
        var moveDir = ctx.ReadValue<float>();

        if (moveDir != 0)
        {
            // turn check
            if (_isFacingRight && moveDir < 0)
            {
                _isFacingRight = false;
                _sprite.transform.Rotate(0, -180f, 0);
            }
            else if (!_isFacingRight && moveDir > 0)
            {
                _isFacingRight = true;
                _sprite.transform.Rotate(0, 180f, 0);
            }

            // Anim
            _animator.SetBool("IsMoving", true);
        }
        else
        {
            // Anim
            _animator.SetBool("IsMoving", false);
        }

        _movement.OnMove(moveDir);
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        var isPressed = ctx.started;

        if (!_isInLight || (_isInLight && !isPressed)) return;

        _movement.OnJump(isPressed);
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (_trigger == null)
        {
            return;
        }

        _trigger.Interact();
        _trigger = null;
    }

    public void OnParry(InputAction.CallbackContext ctx)
    {
        if (_parryingTimer > 0) return;
        if (!_isInLight) return;

        InputManager.Instance.SetInputEnabled(Maps.Player, false);

        Animator.Play(PlayerAnimator.AnimState.Parry);
        _parryingTimer = _parryingAllowTime;

        try
        {
            _parryingTokenSource?.Cancel();
            _parryingTokenSource?.Dispose();
        }
        finally
        {
            _parryingTokenSource = new();
        }

        OnParryEnd(_parryingAllowTime, _parryingTokenSource.Token).Forget();
    }

    #endregion

    private async UniTaskVoid OnParryEnd(float invokeDelay, CancellationToken token)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_parryingAllowTime), cancellationToken: _parryingTokenSource.Token);

        Animator.Stop(PlayerAnimator.AnimState.Parry);
        Animator.Stop(PlayerAnimator.AnimState.Kick);

        InputManager.Instance.SetInputEnabled(Maps.Player, true);

        Time.timeScale = 1;
    }

    public void GetDamaged(int damage, GameObject damageSource)
    {
        if (_invincibleTimer > 0) return;
        if (_isDead) return;

        if (damageSource.TryGetComponent(out Enemy enemy) && _parryingTimer > 0)
        {
            _parryingTimer = 0f;
            Animator.Play(PlayerAnimator.AnimState.Kick);
            SoundManager.Instance.PlaySFX(Path.SFX.Player.Parry, true);
            _invincibleTimer = _invincibleDuration;

            Time.timeScale = 0.5f;

            try
            {
                _parryingTokenSource?.Cancel();
                _parryingTokenSource?.Dispose();
            }
            finally
            {
                _parryingTokenSource = new();
            }

            OnParryEnd(0.5f, _parryingTokenSource.Token).Forget();

            enemy.GetDamaged(3, gameObject);
            EffectManager.Instance.PlayParticle(Effects.Spit, enemy.transform.position + Vector3.up);
            return;
        }

        onPlayerHitBefore?.Invoke(enemy.gameObject);

        ChangeHP(damage);

        if (CurHP > 0)
        {
            SoundManager.Instance.PlaySFX(Path.SFX.Player.Dead);
        }
        else
        {
            _isDead = true;

            GameManager.Instance.GameOver();

            Animator.Play(PlayerAnimator.AnimState.Death, 0.25f);

            SoundManager.Instance.PlaySFX(Path.SFX.Player.Dead);
        }
    }

    public void ChangeHP(int amount)
    {
        int newHP = CurHP + amount;

        newHP = Mathf.Clamp(newHP, 0, MaxHP);

        if (newHP != CurHP)
        {
            CurHP = newHP;
            onPlayerHPChanged?.Invoke();
        }
    }

    public void IncreaseMaxHealth(int amount = 1)
    {
        if (MaxHP >= _stat.UpgradableMaxHP) return;

        MaxHP = Mathf.Min(MaxHP + amount, _stat.UpgradableMaxHP);
        onPlayerHPChanged?.Invoke();
    }

    public void CanInteract(bool _val, IInteractable _trigger = null)
    {
        this._trigger = _trigger;
    }

    private void CountTimers()
    {
        _invincibleTimer -= Time.deltaTime;
        _parryingTimer -= Time.deltaTime;
    }

    public void OnEnterLight()
    {
        _sprite.color = Color.white;

        _movement.ChangeModifier(new PlayerMovementModifier() { MaxMoveSpeed = 1f });
        _isInLight = true;
    }

    public void OnExitLight()
    {
        _sprite.color = Color.red;

        _movement.ChangeModifier(new PlayerMovementModifier() { MaxMoveSpeed = 0.3f });
        _isInLight = false;
    }
}
