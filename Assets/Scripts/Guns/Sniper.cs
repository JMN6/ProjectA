
using UnityEngine;

public class Sniper : Gun
{
    public Sniper(float cooltime, int bullets) : base(cooltime, bullets)
    {
    }

    protected override void Shoot(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapPoint(position, TargetLayerMask);

        if (hit && hit.TryGetComponent(out IDamagalbe target))
        {
            target.GetDamaged(1);
        }
    }
}