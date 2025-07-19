using UnityEngine;

public abstract class Gun
{
    public float Cooltime { get; protected set; }
    public int CurBullets { get; protected set; }
    public int MaxBullets { get; protected set; }

    public Gun(float cooltime, int bullets)
    {
        Cooltime = cooltime;
        CurBullets = bullets;
        MaxBullets = bullets;
    }

    protected static readonly int TargetLayerMask = LayerMask.GetMask("Enemy", "InteractableObject");

    public abstract void Shoot(Vector2 Position);

    public abstract void Recharge();
}