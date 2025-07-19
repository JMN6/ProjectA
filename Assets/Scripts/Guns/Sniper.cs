using UnityEngine;

public class Sniper : Gun
{
    public override void Recharge()
    {
        CurBullets = MaxBullets;
    }

    public override void Shoot(Vector2 position)
    {
        if (CurBullets > 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(position, position, TargetLayerMask);

            if (!hit) return;
            if (hit.collider.TryGetComponent(out IDamagalbe target)) {
                target.GetDamaged(1);
            }
        }
    }


}