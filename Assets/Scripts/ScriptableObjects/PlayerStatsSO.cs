using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsSO", menuName = "Scriptable Objects/PlayerStatsSO")]
public class PlayerStatsSO : ScriptableObject
{
    [field: Header("HP"), SerializeField] public int DefaultMaxHP { get; private set; }
    [field: SerializeField] public int UpgradableMaxHP { get; private set; }

    [field: Header("Gun"), SerializeField] public int _initialLoadedBullets { get; private set; }
    [field: SerializeField] public int _initialAmmo { get; private set; }
    [field: SerializeField] public int MaxMagazine { get; private set; }
    [field: SerializeField] public int MaxAmmo { get; private set; }

    [field: Header("Collision Checks"), SerializeField] public LayerMask GroundMask { get; private set; }
    [field: SerializeField] public float CollisionDetectionLength { get; private set; }

    [field: Header("Run"), Range(1f, 100f), SerializeField] public float MaxMoveSpeed { get; private set; }
    [field: Range(0.25f, 50f), SerializeField] public float GroundAcceleration { get; private set; }
    [field: Range(0.25f, 50f), SerializeField] public float GroundDeceleration { get; private set; }
    [field: Range(0.25f, 50f), SerializeField] public float AirAcceleration { get; private set; }
    [field: Range(0.25f, 50f), SerializeField] public float AirDeceleration { get; private set; }

    [field: Header("Jump"), SerializeField] public float JumpPower { get; private set; }
    [field: SerializeField] public float Gravity { get; private set; }

    [field: Header("More Jump"), SerializeField] public int MaxJumpCount { get; private set; }
    [field: Range(0.01f, 1f), SerializeField] public float ApexThreshold { get; private set; }
    [field: Range(0.01f, 3f), SerializeField] public float ApexHangTime { get; private set; }

    [field: Header("Better Jump"), Range(0.01f, 1f), SerializeField] public float CoyoteAllowTime { get; private set; }
    [field: Range(0.01f, 1f), SerializeField] public float JumpBufferAllowTime { get; private set; }
    [field: Range(1f, 100f), SerializeField] public float MaxFallSpeed { get; private set; }

    [field: Header("Jump Cut"), SerializeField] public float FastFallPower { get; private set; }
    [field: Range(1f, 3f), SerializeField] public float FastFallSpeed { get; private set; }

}
