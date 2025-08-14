using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Input;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour, ILightSensitive
{
    [SerializeField] PlayerStatsSO _stat;
    int _curHP;
    int _maxHP;

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

    public event Action<int> onPlayerHPChanged;


    private void Awake()
    {
        Animator = new(GetComponent<Animator>());

    }

    private void Start()
    {
        _maxHP = _stat.DefaultMaxHP;
        ResetPlayer(transform.position, false);

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
        _curHP = _maxHP;
        onPlayerHPChanged?.Invoke(_curHP);

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

    public bool TryAttack()
    {
        if (_invincibleTimer > 0) return true;
        if (_isDead) return true;

        if (_parryingTimer > 0)
        {
            _parryingTimer = 0f;
            Animator.Play(PlayerAnimator.AnimState.Kick);
            SoundManager.Instance.PlaySFX(Path.PlayerSFX.Parry, true);
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

            return false;
        }
        else
        {
            onPlayerHPChanged?.Invoke(--_curHP);

            if (_curHP > 0)
            {
                SoundManager.Instance.PlaySFX(Path.PlayerSFX.Dead);
            }
            else
            {
                _isDead = true;

                GameManager.Instance.GameOver();

                Animator.Play(PlayerAnimator.AnimState.Death, 0.25f);

                SoundManager.Instance.PlaySFX(Path.PlayerSFX.Dead);
            }

            return true;
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDamagalbe target))
        {
            target.GetDamaged(1);
        }
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
