using Assets.Scripts.Managers;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class TextSetter : MonoBehaviour
{
    private TextMeshProUGUI textMeshUI;
    private Text text;
    [SerializeField] private string textID;

    private void Start()
    {
        LanguageManager.Instance.OnLangChanged.RemoveListener(SetLanguage);
        LanguageManager.Instance.OnLangChanged.AddListener(SetLanguage);

        textMeshUI = GetComponent<TextMeshProUGUI>();
        text = GetComponent<Text>();

        SetLanguage(LanguageManager.Instance.GetCurrentLanguage());
    }

    private void SetLanguage(LanguageManager.LanguageType type)
    {
        if(textMeshUI != null)
        {
            LanguageManager.Instance.SetTextFont(textMeshUI);
            textMeshUI.text = LanguageManager.Instance.GetString(textID, type);
        }

        if(text != null)
        {
            LanguageManager.Instance.SetTextFont(text);
            text.text = LanguageManager.Instance.GetString(textID, type);
        }
    }
}
