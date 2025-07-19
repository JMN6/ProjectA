using TMPro;
using UnityEngine;

public class BulletUI : MonoBehaviour
{
    [SerializeField] private Aim aim;
    [SerializeField] private TextMeshProUGUI bullet;

    private void Update()
    {
        Gun gun = aim.CurGun;

        bullet.text = $"{gun.CurBullets}/{gun.MaxBullets}";
    }
}
