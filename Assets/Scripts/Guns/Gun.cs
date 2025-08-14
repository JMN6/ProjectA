using UnityEngine;

public class Gun
{
    public float Cooltime { get; protected set; }
    public int CurBullets { get; protected set; }
    public int MaxBullets { get; protected set; }

    public PlayerStatsSO _temp;

    private float lastShot;

    private CameraFunctions cameraFunctions = Camera.main.GetComponent<CameraFunctions>();

    protected static readonly int TargetLayerMask = LayerMask.GetMask("Enemy", "InteractableObject");

    private bool isCooldownReady => Time.time - lastShot >= Cooltime;
    private bool isEmpty => CurBullets <= 0;
    public Gun(float cooltime, PlayerStatsSO temp)
    {
        Cooltime = cooltime;
        _temp = temp;
        CurBullets = _temp._initialLoadedBullets;
        MaxBullets = _temp._initialAmmo;
    }

    public void Fire(Vector2 position)
    {
        if (!isCooldownReady) return;

        if (isEmpty)
        {
            SoundManager.Instance.PlaySFX(Path.GunSFX.SniperSFX.Empty, true);
        }
        else
        {
            Shoot(position);

            CurBullets--;
            lastShot = Time.time;

            SoundManager.Instance.PlaySFX(Path.GunSFX.SniperSFX.Shoot, true);
            cameraFunctions.Shake();
        }
    }

    public void Reload()
    {
        SoundManager.Instance.PlaySFX(Path.GunSFX.SniperSFX.Reload);

        int amount = MaxBullets >= _temp.MaxMagazine ? _temp.MaxMagazine : MaxBullets;

        MaxBullets -= amount;
        CurBullets = amount;
    }

    public void Shoot(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapPoint(position, TargetLayerMask);

        if (hit && hit.TryGetComponent(out IDamagalbe target))
        {
            target.GetDamaged(1);
        }
    }
}