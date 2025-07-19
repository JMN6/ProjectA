using UnityEngine;

public class Sniper : Gun
{
    public override void Recharge()
    {
        curBullets = maxBullets;
    }

    public override void Shoot(Vector2 position)
    {
        if (isBulletEnough)
        {
            RaycastHit2D hit = Physics2D.Raycast(position, position, TargetLayerMask);

            if (!hit) return;
            if (hit.collider.TryGetComponent(out IDamagalbe target)) {
                target.GetDamaged(1);
            }
        }
        else
        {
            Recharge();
        }
    }


}