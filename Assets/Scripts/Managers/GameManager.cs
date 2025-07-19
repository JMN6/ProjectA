using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private OptionUI optionUI;

    [field: SerializeField] public OptionSO Option { get; private set; }

    private float tempTimeScale = 1;

    public void OnESC()
    {
        optionUI.Toggle();

        if (Time.timeScale == 0)
        {
            Time.timeScale = tempTimeScale;
        }
        else
        {
            Time.timeScale = 0;
        }
    }

    public void ChangeOption(OptionSO.Option option)
    {
        Option.ChangeOption(option);
    }
}
