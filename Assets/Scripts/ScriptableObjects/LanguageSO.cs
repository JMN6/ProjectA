using Assets.Scripts.Managers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "LanguageSO", menuName = "Scriptable Objects/LanguageSO")]
public class LanguageSO : ScriptableObject
{
    [Serializable]
    public class LanguageSettings
    {
        [SerializeField] public LanguageManager.LanguageType languageType;
        [SerializeField] public TMP_FontAsset FontAsset;
        [SerializeField] public Font Font;
    }

    [SerializeField] public LanguageSettings[] LanguageSets =
        new LanguageSettings[(int)LanguageManager.LanguageType.MAX];
}
