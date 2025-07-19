using System;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private OptionUI optionUI;

    [field: SerializeField] public OptionSO Option { get; private set; }

    private float tempTimeScale = 1;

    public void OnESC()
    {
        if (Time.timeScale == 0)
        {
            optionUI.Hide();

            InputManager.Instance.HideCursor();
            InputManager.Instance.SetInputs(true);
            Time.timeScale = tempTimeScale;
        }
        else
        {
            optionUI.Show();

            InputManager.Instance.ShowCursor();
            InputManager.Instance.SetInputs(false);
            Time.timeScale = 0;
        }
    }

    public void ChangeOption(OptionSO.Option option)
    {
        Option.ChangeOption(option);
    }
}
