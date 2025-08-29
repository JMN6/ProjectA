using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class HUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _tempHP;
    [SerializeField] private Transform _heartParent;
    [SerializeField] private GameObject _heartUI;
    private List<HeartUI> _hearts = new List<HeartUI>();
    private int _currentMaxHP = 0;
    private int _currentHP = 0;

    [SerializeField] TextMeshProUGUI _ammo;

    private void Start()
    {
        GameManager.Instance.Player.onPlayerHPChanged += RefreshHP;
        GameManager.Instance.Aim.onAimAmmoChanged += RefreshAmmo;

        _currentMaxHP = 0;
        _currentHP = GameManager.Instance.Player.CurHP;

        RefreshHP();
    }

    private void RefreshHP()
    {
        int maxHP = GameManager.Instance.Player.MaxHP;
        int currentHP = GameManager.Instance.Player.CurHP;

        for (int i = _currentMaxHP; i < maxHP; ++i)
        {
            _hearts.Add(Instantiate(_heartUI, _heartParent).GetComponent<HeartUI>());
        }
        _currentMaxHP = maxHP;

        for(int i = 0; i < maxHP; ++i)
        {
            _hearts[i].SetHeart(i >= currentHP);
        }
    }

    private void RefreshAmmo()
    {
        _ammo.text = $"Ammo ¡¿ {GameManager.Instance.Aim.Ammo}";
    }
}