using UnityEngine;

public abstract class Gun
{
    protected float cooltime;
    protected float curBullets;
    protected float maxBullets = 5;
    protected bool isBulletEnough => curBullets > 0;

    protected static readonly int TargetLayerMask = LayerMask.GetMask("Enemy", "InteractableObject");

    public abstract void Shoot(Vector2 Position);

    public abstract void Recharge();
}