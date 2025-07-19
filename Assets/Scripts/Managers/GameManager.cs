using System;
using UnityEngine;
using System.Collections;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private Transform player;
    private Player playerScript;

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
    [SerializeField] private CutScenePlayer cutScenePlayer;
    //[SerializeField] private bool isCutScenePlay = false;

    [Header("Camara")]
    [SerializeField] private CameraFunctions camFunc;

    [Header("DeadEffect")]
    [SerializeField] private float zoomInTime = 1.3f;
    [SerializeField] private float zoomInScale = 0.5f;
    [SerializeField] private float zoomOutTime = 0.5f;
    [SerializeField] private float zoomOutScale = 1f;

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
        //cutScenePlayer.StartCutScene(CutSceneParser.Convert(ClearCutScene), ReturnToMain);
    }

    public void GameOver()
    {
        InputManager.Instance.SetInputs(false);

        camFunc.Zoom(zoomInTime, zoomInScale, ResetGame);
    }

    private void ResetGame()
    {
        if (playerScript == null)
        {
            playerScript = player.GetComponent<Player>();
        }

        playerScript.ResetPlayer();

        player.position = datas[currentStage].respawnPosition.position;

        foreach (var _ in datas[currentStage].ResetObjs)
        {
            _.SetActive(false);
            _.SetActive(true);
        }

        camFunc.Zoom(zoomOutTime, zoomOutScale, GameAllSet);
    }

    private void GameAllSet()
    {
        InputManager.Instance.SetInputs(true);
    }
}
