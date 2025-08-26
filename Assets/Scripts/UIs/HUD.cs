using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _tempHP;
    [SerializeField] TextMeshProUGUI _ammo;

    private void Start()
    {
        GameManager.Instance.Player.onPlayerHPChanged += RefreshHP;
        GameManager.Instance.Aim.onAimAmmoChanged += RefreshAmmo;
        RefreshHP();
    }

    private void RefreshHP()
    {
        string hp = string.Empty;

        for(int i = 0; i < GameManager.Instance.Player.MaxHP; i++)
        {
            hp += GameManager.Instance.Player.CurHP > i ? "��" : "��";
        }

        _tempHP.text = $"HP {hp}";
    }

    private void RefreshAmmo()
    {
        _ammo.text = $"Ammo �� {GameManager.Instance.Aim.Ammo}";
    }
}