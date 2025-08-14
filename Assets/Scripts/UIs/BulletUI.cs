using TMPro;
using UnityEngine;

public class BulletUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bullet;

    private void Awake()
    {
        GameManager.Instance.Aim.CurGun.onAmmoChanged += Refresh;
    }

    private void Refresh(int loadedBullet, int ammo)
    {
        bullet.text = $"{loadedBullet}/{ammo}";
    }
}
