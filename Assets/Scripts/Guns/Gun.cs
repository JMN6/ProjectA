using System;
using UnityEngine;

public class Gun
{
    public float Cooltime { get; protected set; }
    public int LoadedBullets { get; protected set; }
    public int Ammo { get; protected set; }

    public PlayerStatsSO _temp;

    private float lastShot;

    private CameraFunctions cameraFunctions = Camera.main.GetComponent<CameraFunctions>();

    protected static readonly int TargetLayerMask = LayerMask.GetMask("Enemy", "InteractableObject");

    public event Action<int, int> onAmmoChanged;

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
        Ammo = _temp._initialAmmo;

        onAmmoChanged?.Invoke(LoadedBullets, Ammo);
    }

    public void ChangeAmmo(int amount)
    {
        int newAmmo = Ammo + amount;

        newAmmo = Mathf.Clamp(newAmmo, 0, _temp.MaxAmmo);

        if (newAmmo != Ammo)
        {
            Ammo = newAmmo;
            onAmmoChanged?.Invoke(LoadedBullets, Ammo);
        }
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

            onAmmoChanged?.Invoke(LoadedBullets, Ammo);
        }
    }

    public void Reload()
    {
        SoundManager.Instance.PlaySFX(Path.SFX.Gun.SniperSFX.Reload);

        int amount = Ammo >= _temp.MaxMagazine ? _temp.MaxMagazine : Ammo;

        LoadedBullets = amount;

        ChangeAmmo(-amount);
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