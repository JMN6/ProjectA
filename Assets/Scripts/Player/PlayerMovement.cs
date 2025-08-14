using UnityEngine;
using Game.Input;
using UnityEngine.InputSystem;
using static Player;

[RequireComponent(typeof(Player), typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Render")]
    [SerializeField] private bool _isFacingRight;
    private Player _player;
    private Rigidbody2D _rigid;

    [Header("Collision Checks")]
    [SerializeField] private Collider2D _bodyCollider;
    [SerializeField] private Collider2D _feetCollider;
    [SerializeField] private LayerMask _groundMask;
    ContactFilter2D _filter;
    [SerializeField] private float _collisionDetectionLength;
    private RaycastHit2D[] _castHits = new RaycastHit2D[1];
    private Rigidbody2D _vehicle;

    [Header("Run")]
    [Range(1f, 100f), SerializeField] private float _maxMoveSpeed;
    [Range(0.25f, 50f), SerializeField] private float _groundAcceleration;
    [Range(0.25f, 50f), SerializeField] private float _groundDeceleration;
    [Range(0.25f, 50f), SerializeField] private float _airAcceleration;
    [Range(0.25f, 50f), SerializeField] private float _airDeceleration;
    [Range(0f, 3f), SerializeField] private float _runTransitionInDuration;
    [Range(0f, 3f), SerializeField] private float _runTransitionOutDuration;
    private float _moveDirection;
    private float _horizontalVelocity;

    [Header("Jump")]
    [SerializeField] private float _jumpPower;
    [SerializeField] private float _gravity;
    private bool _isGrounded;
    private bool _isJumping;
    private bool _isFalling;
    private float _verticalVelocity;

    [Header("More Jump")]
    [SerializeField] private int _maxJumpCount = 1;
    [Range(0.01f, 1f), SerializeField] private float _apexThreshold;
    [Range(0.01f, 3f), SerializeField] private float _apexHangTime;
    private int _curJumpCount;
    private float _apexPoint;
    private float _apexHangTimer;
    private bool _isInApex;

    [Header("Better Jump")]
    [Range(0.01f, 1f), SerializeField] private float _coyoteAllowTime;
    [Range(0.01f, 1f), SerializeField] private float _jumpBufferAllowTime;
    [Range(1f, 100f), SerializeField] private float _maxFallSpeed;
    private float _coyoteTimer;
    private float _jumpBufferTimer;

    [Header("Jump Cut")]
    [SerializeField] private float _fastFallPower;
    [Range(1f, 3f), SerializeField] private float _fastFallSpeed;
    private bool _isFastFalling;
    private bool _isHeadBumped;
    private bool _isJumpReleased;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _rigid = GetComponent<Rigidbody2D>();

        _filter = new ContactFilter2D();
        _filter.SetLayerMask(_groundMask); // 기존 레이어 마스크 적용
        _filter.useLayerMask = true;
        _filter.useTriggers = false; // 트리거 무시 (원하면 true로 변경)
    }

    private void Start()
    {
        var input = InputManager.Instance;

        input.AddListener(Actions.Move, TriggerFlag.Performed | TriggerFlag.Canceled, OnMove);
        input.AddListener(Actions.Jump, TriggerFlag.Performed | TriggerFlag.Canceled, OnJump);
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


    #region Move
    private void Move()
    {
        if (_moveDirection != 0)
        {
            var acc = _isGrounded ? _groundAcceleration : _airAcceleration;

            _horizontalVelocity = Mathf.Lerp(_horizontalVelocity, _moveDirection * _maxMoveSpeed, acc * Time.deltaTime);

            // Sound
            if (_isGrounded)
                SoundManager.Instance.PlaySFX(Path.PlayerSFX.Walk);
            else
                SoundManager.Instance.StopSFX(Path.PlayerSFX.Walk);
        }
        else
        {
            var dec = _isGrounded ? _groundDeceleration : _airDeceleration;
            _horizontalVelocity = Mathf.Lerp(_horizontalVelocity, 0f, dec * Time.deltaTime);

            // Sound
            SoundManager.Instance.StopSFX(Path.PlayerSFX.Walk);
        }
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        _moveDirection = ctx.ReadValue<float>();

        if (_moveDirection != 0)
        {
            // turn check
            if (_isFacingRight && _moveDirection < 0) Turn(false);
            else if (!_isFacingRight && _moveDirection > 0) Turn(true);

            // Anim
            _player.Animator.Play(PlayerAnimator.AnimState.Run, _runTransitionInDuration);
        }
        else
        {
            // Anim
            _player.Animator.Stop(PlayerAnimator.AnimState.Run, _runTransitionOutDuration);
        }
    }

    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            _isFacingRight = true;
            transform.Rotate(0, 180f, 0);
        }
        else
        {
            _isFacingRight = false;
            transform.Rotate(0, -180f, 0);
        }
    }
    #endregion

    #region Jump & Fall & Land
    private void DoJump()
    {
        _isJumping = true;
        _jumpBufferTimer = 0f;
        _coyoteTimer = 0f;
        _verticalVelocity = _jumpPower;
        _curJumpCount++;

        _player.Animator.Stop(PlayerAnimator.AnimState.Fall);
        _player.Animator.Play(PlayerAnimator.AnimState.Jump);
        SoundManager.Instance.PlaySFX(Path.PlayerSFX.Jump, true);
    }

    private void Jump()
    {
        // Jump Checks
        if (_jumpBufferTimer > 0 && (_curJumpCount < _maxJumpCount || _isGrounded || _coyoteTimer > 0))
        {
            DoJump();
        }

        // Under Apex
        if (!_isInApex && _isJumping && _verticalVelocity >= 0)
        {
            if (_isHeadBumped)
            {
                _isFastFalling = true;
                _verticalVelocity = -_fastFallPower;
            }

            if (_isJumpReleased)
            {
                _isFastFalling = true;
                _verticalVelocity -= _fastFallPower;
            }

            _apexPoint = Mathf.InverseLerp(_jumpPower, 0, _verticalVelocity);

            if (_apexPoint > _apexThreshold)
            {
                _isInApex = true;
                _apexHangTimer = _apexHangTime;
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
                _verticalVelocity += -_gravity * _fastFallSpeed * Time.deltaTime;
            }
            else
            {
                _verticalVelocity += -_gravity * Time.deltaTime;
            }

            _verticalVelocity = Mathf.Clamp(_verticalVelocity, -_maxFallSpeed, 20f);
        }
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started || ctx.performed)
        {
            _jumpBufferTimer = _jumpBufferAllowTime;
            _isJumpReleased = false;
        }
        else if (ctx.canceled)
        {
            _isJumpReleased = true;
        }
    }

    private void GroundChecks()
    {
        Vector2 boxCastOrigin = new Vector2(_feetCollider.bounds.center.x, _feetCollider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetCollider.bounds.size.x, _collisionDetectionLength);

        int hits = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, _filter, _castHits, _collisionDetectionLength);

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

#if UNITY_EDITOR
        Color rayColor = _isGrounded ? Color.green : Color.red;

        Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * _collisionDetectionLength, rayColor);
        Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * _collisionDetectionLength, rayColor);
        Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.right * boxCastSize.x, rayColor);
        Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - _collisionDetectionLength), Vector2.right * boxCastSize.x, rayColor);
#endif
    }
    private void CeilingChecks()
    {
        Vector2 boxCastOrigin = new Vector2(_bodyCollider.bounds.center.x, _bodyCollider.bounds.max.y);
        Vector2 boxCastSize = new Vector2(_feetCollider.bounds.size.x, _collisionDetectionLength);

        int hits = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, _filter, _castHits, _collisionDetectionLength);

        if (hits > 0 && !_castHits[0].transform.TryGetComponent(out PlatformEffector2D platform))
        {
            _isHeadBumped = true;
        }
        else
        {
            _isHeadBumped = false;
        }

#if UNITY_EDITOR
        Color rayColor = _isHeadBumped ? Color.green : Color.red;

        Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.up * _collisionDetectionLength, rayColor);
        Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.up * _collisionDetectionLength, rayColor);
        Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.right * boxCastSize.x, rayColor);
        Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y + _collisionDetectionLength), Vector2.right * boxCastSize.x, rayColor);
#endif
    }
    #endregion

    private void CountTimers()
    {
        _jumpBufferTimer -= Time.deltaTime;
        _apexHangTimer -= Time.deltaTime;

        if (_isGrounded)
        {
            _coyoteTimer = _coyoteAllowTime;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }
    }
}