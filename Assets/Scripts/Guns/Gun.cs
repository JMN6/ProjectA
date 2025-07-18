using UnityEngine;

public abstract class Gun
{
    [SerializeField] protected float cooltime;
    [SerializeField] protected float curBullets;
    [SerializeField] protected float maxBullets;
    protected bool isBulletEnough => curBullets > 0;

    public abstract void Shoot(Vector2 Position);
}