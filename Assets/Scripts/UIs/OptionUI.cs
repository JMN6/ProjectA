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
        closeBtn.onClick.AddListener(Toggle);
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
    }

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
