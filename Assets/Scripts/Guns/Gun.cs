using UnityEngine;

public abstract class Gun
{
    protected float cooltime;
    protected float curBullets;
    protected float maxBullets = 5;
    protected bool isBulletEnough => curBullets > 0;

    public abstract void Shoot(Vector2 Position);

    public abstract void Recharge();
}