using System;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private Transform player;

    [Header("Option")]
    [SerializeField] private OptionUI optionUI;

    [field: SerializeField] public OptionSO Option { get; private set; }

    [System.Serializable]
    private class StageDatas
    {
        public Transform respawnPosition;
        public GameObject[] ResetObjs;
    }

    [Header("Stage")]
    [SerializeField] private int maxStage = 3;
    [SerializeField] private int currentStage = 0;
    [SerializeField] private StageDatas[] datas;

    [Header("CutScene")]
    [SerializeField] private string ClearCutScene = "TempCutScene";
    [SerializeField] private CutScenePlayer cutScenePlayer;

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

    public void StageClear()
    {
        currentStage = Mathf.Clamp(currentStage + 1, 0, maxStage);
        if(currentStage == maxStage)
        {
            GameClear();
            return;
        }


    }

    private void GameClear()
    {
        cutScenePlayer.StartCutScene(CutSceneParser.Convert(ClearCutScene), ReturnToMain);
    }

    private void ReturnToMain()
    {
        // 여기서 엔딩 컷씬 이후 로직 추가
    }

    public void ResetGame()
    {
        player.position = datas[currentStage].respawnPosition.position;

        foreach(var _ in datas[currentStage].ResetObjs)
        {
            _.SetActive(false);
            _.SetActive(true);
        }
    }
}
