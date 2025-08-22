using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _tempHP;

    private void Start()
    {
        GameManager.Instance.Player.onPlayerHPChanged += RefreshHP;
    }

    private void RefreshHP(int curHP, int maxHP)
    {
        string hp = string.Empty;

        for(int i = 0; i < maxHP; i++)
        {
            hp += curHP > i ? "●" : "○";
        }

        _tempHP.text = $"HP {hp}";
    }
}