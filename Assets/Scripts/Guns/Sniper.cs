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

        RaycastHit2D[] hits = Physics2D.RaycastAll(position, position, TargetLayerMask);

        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent(out IDamagalbe target))
            {
                target.GetDamaged(1);
                EffectManager.Instance.PlayParticle(0, position);
                return;
            }
        }
    }

}