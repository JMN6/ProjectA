using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSettingUI : MonoBehaviour
{
    [SerializeField] private Button langSetButton;

    [SerializeField] private GameObject langSetPanel;
    [SerializeField] private Button[] langBtns = new Button[(int)LanguageManager.LanguageType.MAX];

    private void Start()
    {
        langSetButton.onClick.RemoveAllListeners();
        langSetButton.onClick.AddListener(() => ShowLangSetPanel(true));

        for (int i = 0, size = langBtns.Length; i < size; ++i)
        {
            LanguageManager.LanguageType type = (LanguageManager.LanguageType)i;
            langBtns[i].onClick.RemoveAllListeners();
            langBtns[i].onClick.AddListener(() => SetLang(type));
        }
    }

    public void ShowLangSetPanel(bool isShown = true)
    {
        langSetPanel.SetActive(isShown);
    }

    public void SetLang(LanguageManager.LanguageType type)
    {
        LanguageManager.Instance.SetLanguageType(type);
        ShowLangSetPanel(false);
    }
}
