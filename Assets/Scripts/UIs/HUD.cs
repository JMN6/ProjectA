using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class HUD : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private Transform _heartParent;
    [SerializeField] private GameObject _heartUI;
    private List<HeartUI> _hearts = new List<HeartUI>();
    private int _currentMaxHP = 0;

    [Header("Ammo")]
    [SerializeField] TextMeshProUGUI _ammo;

    private void Start()
    {
        GameManager.Instance.Player.onPlayerHPChanged += RefreshHP;
        GameManager.Instance.Aim.onAimAmmoChanged += RefreshAmmo;

        _currentMaxHP = 0;

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
        _ammo.text = $"X {GameManager.Instance.Aim.Ammo}";
    }
}