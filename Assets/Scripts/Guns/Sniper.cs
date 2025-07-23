using UnityEngine;

public class Sniper : Gun
{
    public Sniper(float cooltime, int bullets) : base(cooltime, bullets)
    {
    }

    public override void Recharge()
    {
        CurBullets = MaxBullets;
    }

    public override void Shoot(Vector2 position)
    {
        CurBullets--;

        Collider2D hit = Physics2D.OverlapPoint(position, TargetLayerMask);

        if (hit && hit.TryGetComponent(out IDamagalbe target))
        {
            target.GetDamaged(1);
            EffectManager.Instance.PlayParticle(0, position);
            return;
        }
    }

}