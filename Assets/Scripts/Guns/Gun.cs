using UnityEngine;

public abstract class Gun
{
    public float Cooltime { get; protected set; }
    public int CurBullets { get; protected set; }
    public int MaxBullets { get; protected set; }

    private float lastShot;

    public Gun(float cooltime, int bullets)
    {
        Cooltime = cooltime;
        CurBullets = bullets;
        MaxBullets = bullets;
    }

    protected static readonly int TargetLayerMask = LayerMask.GetMask("Enemy", "InteractableObject");

    private bool isCooldownReady => Time.time - lastShot >= Cooltime;

    public void Shoot(Vector2 position)
    {
        if (!isCooldownReady) return;

        Collider2D hit = Physics2D.OverlapPoint(position, TargetLayerMask);

        if (hit && hit.TryGetComponent(out IDamagalbe target))
        {
            target.GetDamaged(1);
        }

        CurBullets--;
        lastShot = Time.time;
    }

    public void Recharge()
    {
        CurBullets = MaxBullets;
    }
}