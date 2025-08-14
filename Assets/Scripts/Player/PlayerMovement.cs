using UnityEngine;
using Game.Input;
using UnityEngine.InputSystem;
using static Player;

public class PlayerMovementModifier
{
    public float MaxMoveSpeed;
}

[RequireComponent(typeof(Player), typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] PlayerStatsSO _stat;
    PlayerMovementModifier _statModifier;

    private Player _player;
    private Rigidbody2D _rigid;

    [Header("Collision Checks")]
    [SerializeField] private Collider2D _bodyCollider;
    [SerializeField] private Collider2D _feetCollider;
    ContactFilter2D _filter;
    private RaycastHit2D[] _castHits = new RaycastHit2D[1];
    private Rigidbody2D _vehicle;

    //[Header("Run")]
    private float _moveDirection;
    private float _horizontalVelocity;

    //[Header("Jump")]
    private bool _isGrounded;
    private bool _isJumping;
    private bool _isFalling;
    private float _verticalVelocity;

    //[Header("More Jump")]
    private int _curJumpCount;
    private float _apexPoint;
    private float _apexHangTimer;
    private bool _isInApex;

    //[Header("Better Jump")]
    private float _coyoteTimer;
    private float _jumpBufferTimer;

    //[Header("Jump Cut")]
    private bool _isFastFalling;
    private bool _isHeadBumped;
    private bool _isJumpReleased;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _rigid = GetComponent<Rigidbody2D>();

        _filter = new ContactFilter2D();
        _filter.SetLayerMask(_stat.GroundMask); // 기존 레이어 마스크 적용
        _filter.useLayerMask = true;
        _filter.useTriggers = false; // 트리거 무시 (원하면 true로 변경)
    }

    private void Update()
    {
        GroundChecks();
        CeilingChecks();
        CountTimers();
        Move();
        Jump();
    }

    private void FixedUpdate()
    {
        var moveVelocity = Vector2.zero;

        if (!_isJumping && _vehicle)
        {
            moveVelocity = new Vector2(_horizontalVelocity + _vehicle.linearVelocityX, _vehicle.linearVelocityY);
        }
        else
        {
            moveVelocity = new Vector2(_horizontalVelocity, _verticalVelocity);
        }
        
        _rigid.linearVelocity = moveVelocity;
    }

    public void ChangeModifier(PlayerMovementModifier modifier)
    {
        _statModifier = modifier;
    }


    #region Move

    public void OnMove(float moveDirection)
    {
        _moveDirection = moveDirection;
    }

    private void Move()
    {
        if (_moveDirection != 0)
        {
            var acc = _isGrounded ? _stat.GroundAcceleration : _stat.AirAcceleration;

            _horizontalVelocity = Mathf.Lerp(_horizontalVelocity, _moveDirection * _stat.MaxMoveSpeed * _statModifier.MaxMoveSpeed, acc * Time.deltaTime);

            // Sound
            if (_isGrounded)
                SoundManager.Instance.PlaySFX(Path.SFX.Player.Walk);
            else
                SoundManager.Instance.StopSFX(Path.SFX.Player.Walk);
        }
        else
        {
            var dec = _isGrounded ? _stat.GroundDeceleration : _stat.AirDeceleration;
            _horizontalVelocity = Mathf.Lerp(_horizontalVelocity, 0f, dec * Time.deltaTime);

            // Sound
            SoundManager.Instance.StopSFX(Path.SFX.Player.Walk);
        }
    }
    #endregion

    #region Jump & Fall & Land
    private void DoJump()
    {
        _isJumping = true;
        _jumpBufferTimer = 0f;
        _coyoteTimer = 0f;
        _verticalVelocity = _stat.JumpPower;
        _curJumpCount++;

        _player.Animator.Stop(PlayerAnimator.AnimState.Fall);
        _player.Animator.Play(PlayerAnimator.AnimState.Jump);
        SoundManager.Instance.PlaySFX(Path.SFX.Player.Jump, true);
    }

    private void Jump()
    {
        // Jump Checks
        if (_jumpBufferTimer > 0 && (_curJumpCount < _stat.MaxJumpCount || _isGrounded || _coyoteTimer > 0))
        {
            DoJump();
        }

        // Under Apex
        if (!_isInApex && _isJumping && _verticalVelocity >= 0)
        {
            if (_isHeadBumped)
            {
                _isFastFalling = true;
                _verticalVelocity = -_stat.FastFallPower;
            }

            if (_isJumpReleased)
            {
                _isFastFalling = true;
                _verticalVelocity -= _stat.FastFallPower;
            }

            _apexPoint = Mathf.InverseLerp(_stat.JumpPower, 0, _verticalVelocity);

            if (_apexPoint > _stat.ApexThreshold)
            {
                _isInApex = true;
                _apexHangTimer = _stat.ApexHangTime;
            }
        }

        // Exit Apex
        if (_isInApex && _apexHangTimer < 0)
        {
            _isInApex = false;
        }

        // Start Falling
        if (!_isGrounded && _verticalVelocity < 0 && !_isFalling && _coyoteTimer < 0)
        {
            _isFalling = true;
            _player.Animator.Play(PlayerAnimator.AnimState.Fall);

            if (!_isJumping)
                _curJumpCount++;
        }

        // Landed
        if ((_isJumping || _isFalling) && _isGrounded && _verticalVelocity < 0.01f)
        {
            _isJumping = false;
            _isFalling = false;
            _isFastFalling = false;
            _curJumpCount = 0;
            _verticalVelocity = 0;

            _player.Animator.Stop(PlayerAnimator.AnimState.Jump);
            _player.Animator.Stop(PlayerAnimator.AnimState.Fall);
        }

        // Ignore Gravity
        if (_isInApex)
        {
            _verticalVelocity = 0;
        }

        // Apply Gravity
        else if (!_isGrounded)
        {
            if (_isFastFalling)
            {
                _verticalVelocity += -_stat.Gravity * _stat.FastFallSpeed * Time.deltaTime;
            }
            else
            {
                _verticalVelocity += -_stat.Gravity * Time.deltaTime;
            }

            _verticalVelocity = Mathf.Clamp(_verticalVelocity, -_stat.MaxFallSpeed, 20f);
        }
    }

    public void OnJump(bool isPressed)
    {
        if (isPressed)
        {
            _jumpBufferTimer = _stat.JumpBufferAllowTime;
            _isJumpReleased = false;
        }
        else
        {
            _isJumpReleased = true;
        }
    }

    private void GroundChecks()
    {
        Vector2 boxCastOrigin = new Vector2(_feetCollider.bounds.center.x, _feetCollider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetCollider.bounds.size.x, _stat.CollisionDetectionLength);

        int hits = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, _filter, _castHits, _stat.CollisionDetectionLength);

        _isGrounded = false;
        _vehicle = null;

        if (hits > 0)
        {
            if (_castHits[0].transform.TryGetComponent(out MovingPlatforms vehicle))
            {
                if (boxCastOrigin.y >= _castHits[0].transform.position.y)
                {
                    _isGrounded = true;
                    _vehicle = vehicle.GetBody();
                }
            }
            else
            {
                _isGrounded = true;
            }
        }
    }
    private void CeilingChecks()
    {
        Vector2 boxCastOrigin = new Vector2(_bodyCollider.bounds.center.x, _bodyCollider.bounds.max.y);
        Vector2 boxCastSize = new Vector2(_feetCollider.bounds.size.x, _stat.CollisionDetectionLength);

        int hits = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, _filter, _castHits, _stat.CollisionDetectionLength);

        if (hits > 0 && !_castHits[0].transform.TryGetComponent(out PlatformEffector2D platform))
        {
            _isHeadBumped = true;
        }
        else
        {
            _isHeadBumped = false;
        }
    }
    #endregion

    private void CountTimers()
    {
        _jumpBufferTimer -= Time.deltaTime;
        _apexHangTimer -= Time.deltaTime;

        if (_isGrounded)
        {
            _coyoteTimer = _stat.CoyoteAllowTime;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }
    }
}