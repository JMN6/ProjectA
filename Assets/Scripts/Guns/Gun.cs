using UnityEngine;

public abstract class Gun
{
    public float Cooltime { get; protected set; }
    public float CurBullets { get; protected set; }
    public float MaxBullets { get; protected set; }

    protected static readonly int TargetLayerMask = LayerMask.GetMask("Enemy", "InteractableObject");

    public abstract void Shoot(Vector2 Position);

    public abstract void Recharge();
}