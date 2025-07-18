using UnityEngine;

public class Sniper : Gun
{
    public override void Shoot(Vector2 position)
    {
        if (isBulletEnough)
        {
            RaycastHit2D hit = Physics2D.Raycast(position, position);

            if (hit.collider.TryGetComponent(out Gun target)) {
                //
            }
        }
    }
}