using UnityEngine;

public abstract class Gun
{
    public float Cooltime { get; protected set; }
    public int CurBullets { get; protected set; }
    public int MaxBullets { get; protected set; }

    private float lastShot;

    private CameraFunctions cameraFunctions = Camera.main.GetComponent<CameraFunctions>();

    public Gun(float cooltime, int bullets)
    {
        Cooltime = cooltime;
        CurBullets = bullets;
        MaxBullets = bullets;
    }

    protected static readonly int TargetLayerMask = LayerMask.GetMask("Enemy", "InteractableObject");

    private bool isCooldownReady => Time.time - lastShot >= Cooltime;
    private bool isEmpty => CurBullets <= 0;

    public void Fire(Vector2 position)
    {
        if (!isCooldownReady) return;

        if (isEmpty)
        {
            SoundManager.Instance.PlaySFX(Path.GunSFX.SniperSFX.Empty);
        }
        else
        {
            Shoot(position);

            CurBullets--;
            lastShot = Time.time;

            SoundManager.Instance.PlaySFX(Path.GunSFX.SniperSFX.Shoot);
            cameraFunctions.Shake();
        }
    }

    public void Reload()
    {
        SoundManager.Instance.PlaySFX(Path.GunSFX.SniperSFX.Reload);

        CurBullets = MaxBullets;
    }

    protected abstract void Shoot(Vector2 position);
}