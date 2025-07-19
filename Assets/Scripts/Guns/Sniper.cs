using UnityEngine;

public class Sniper : Gun
{
    public override void Recharge()
    {
        CurBullets = MaxBullets;
    }

    public override void Shoot(Vector2 position)
    {
        if (CurBullets == 0)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(position, position, TargetLayerMask);

            foreach (var hit in hits)
            {
                if (hit.collider.TryGetComponent(out IDamagalbe target))
                {
                    target.GetDamaged(1);
                    return;
                }
            }
        }
    }


}