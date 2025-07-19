using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "OptionSO", menuName = "Data/OptionSO")]
public class OptionSO : ScriptableObject
{
    [System.Serializable]
    public struct Option
    {
        [Range(0.3f, 1f)]
        public float MouseSensitivity;

        [Range(0f, 1f)]
        public float BgmVolume;

        [Range(0f, 1f)]
        public float SfxVolume;

        public Option(float mouseSensitivity, float bgmVolume, float sfxVolume)
        {
            MouseSensitivity = mouseSensitivity;
            BgmVolume = bgmVolume;
            SfxVolume = sfxVolume;
        }
    }

    [SerializeField]
    private Option option;

    public float MouseSensitivity => option.MouseSensitivity;
    public float BgmVolume => option.BgmVolume;
    public float SfxVolume => option.SfxVolume;

    public void ChangeOption(Option option)
    {
        this.option = option;
    }
}
