using TMPro;
using UnityEngine;

public class BulletUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bullet;

    private void Awake()
    {
        GameManager.Instance.Aim.CurGun.onGunAmmoChanged += Refresh;
    }

    private void Refresh()
    {
        bullet.text = $"{GameManager.Instance.Aim.CurGun.LoadedBullets}/5";
    }
}
