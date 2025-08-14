using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _tempHP;

    private void Start()
    {
        FindFirstObjectByType<Player>().onPlayerHPChanged += RefreshHP;
        RefreshHP(3);
    }

    private void RefreshHP(int curHP)
    {
        string hp = string.Empty;

        for(int i = 0; i < 3; i++)
        {
            hp += curHP > i ? "¡Ü" : "¡Û";
        }

        _tempHP.text = $"HP {hp}";
    }
}