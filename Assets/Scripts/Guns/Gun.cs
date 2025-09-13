using System;
using UnityEngine;

public class Gun
{
    public float Cooltime { get; protected set; }
    public int LoadedBullets { get; protected set; }
    public const int MaxBullets = 5;

    public PlayerStatsSO _temp;

    private float lastShot;

    private CameraFunctions cameraFunctions = Camera.main.GetComponent<CameraFunctions>();

    protected static readonly int TargetLayerMask = LayerMask.GetMask("Enemy", "InteractableObject", "EyeEnemy");

    public event Action onGunAmmoChanged;

    private bool isCooldownReady => Time.time - lastShot >= Cooltime;
    private bool isEmpty => LoadedBullets <= 0;
    public Gun(float cooltime, PlayerStatsSO temp)
    {
        Cooltime = cooltime;
        _temp = temp;
    }

    public void Init()
    {
        LoadedBullets = _temp._initialLoadedBullets;

        onGunAmmoChanged?.Invoke();
    }

    public void Fire(Vector2 position)
    {
        if (!isCooldownReady) return;

        if (isEmpty)
        {
            SoundManager.Instance.PlaySFX(Path.SFX.Gun.SniperSFX.Empty, true);
        }
        else
        {
            Shoot(position);

            LoadedBullets--;
            lastShot = Time.time;

            SoundManager.Instance.PlaySFX(Path.SFX.Gun.SniperSFX.Shoot, true);
            cameraFunctions.Shake();

            onGunAmmoChanged?.Invoke();
        }
    }

    public void Reload()
    {
        SoundManager.Instance.PlaySFXRandom(Path.SFX.Gun.SniperSFX.Reload, Path.SFX.Gun.SniperSFX.ReloadSoundsCount, true);

        LoadedBullets = MaxBullets;
        onGunAmmoChanged?.Invoke();

    }

    public void Shoot(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapPoint(position, TargetLayerMask);

        if (hit && hit.TryGetComponent(out IDamageable target))
        {
            target.GetDamaged(1, GameManager.Instance.Aim.gameObject);
        }
    }
}