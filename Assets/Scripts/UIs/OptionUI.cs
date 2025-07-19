using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [SerializeField] private Slider mouseSensitivity;
    [SerializeField] private Slider bgmVolume;
    [SerializeField] private Slider sfxVolume;

    [SerializeField] private Button closeBtn;

    private void Awake()
    {
        closeBtn.onClick.AddListener(GameManager.Instance.OnESC);
    }

    private void OnEnable()
    {
        var option = GameManager.Instance.Option;

        mouseSensitivity.value = option.MouseSensitivity;
        bgmVolume.value = option.BgmVolume;
        sfxVolume.value = option.SfxVolume;
    }
    private void Update()
    {
        GameManager.Instance.ChangeOption(new OptionSO.Option(mouseSensitivity.value, bgmVolume.value, sfxVolume.value));

        SoundManager.Instance.SetBGMVolume(bgmVolume.value);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
