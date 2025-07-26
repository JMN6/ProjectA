using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DataParseAndPlayer;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private Transform player;
    private Player _playerScript;
    public Player PlayerScript
    {
        get
        {
            if (_playerScript == null)
            {
                _playerScript = player?.GetComponent<Player>();
            }
            return _playerScript;
        }
        set { _playerScript = value; }
    }

    [Header("Option")]
    [SerializeField] private OptionUI optionUI;

    [field: SerializeField] public OptionSO Option { get; private set; }

    [Header("CutScene")]
    [SerializeField] private CutScenePlayer cutScenePlayer;
    [SerializeField] private string clearCutScene;

    [Header("Camara")]
    [SerializeField] private CameraFollow camFollow;
    [SerializeField] private CameraFunctions camFunc;

    [Header("DeadEffect")]
    [SerializeField] private float zoomInTime = 1.3f;
    [SerializeField] private float zoomInScale = 0.5f;
    [SerializeField] private float zoomOutTime = 0.5f;
    [SerializeField] private float zoomOutScale = 1f;

    [Header("UIInputs")]
    [SerializeField] private UnityEvent Skip;

    [Header("Stage")]
    [SerializeField] private StageManager stageManager;

    [SerializeField] private int currentStage = 0;
    private int maxStage;

    private float tempTimeScale = 1;

    public void Start()
    {
        camFollow.OpenningSequence(player);
        maxStage = stageManager.StagetCount;
    }

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

    public void OnSkip()
    {
        Skip?.Invoke();

    }
    public IResetable[] ResetScripts;

    public void ChangeOption(OptionSO.Option option)
    {
        Option.ChangeOption(option);
    }

    public void StageClear()
    {
        currentStage = Mathf.Clamp(currentStage + 1, 0, maxStage);
        if (currentStage == maxStage)
        {
            GameClear();
            return;
        }
    }

    private void GameClear()
    {
        cutScenePlayer.StartScene(Parser<CutSceneRow>.Convert("CutScene", clearCutScene), ReturnToMain);
    }

    public void GameOver()
    {
        InputManager.Instance.SetInputs(false);

        camFunc.Zoom(zoomInTime, zoomInScale, ResetGame);
    }

    private void GameAllSet()
    {
        InputManager.Instance.SetInputs(true);
    }

    private void ResetGame()
    {
        PlayerScript.ResetPlayer(stageManager.GetRespawnPosition(currentStage), 
            stageManager.GetRespawnFaceWay(currentStage) );

        stageManager.ResetGame(currentStage);

        camFunc.Zoom(zoomOutTime, zoomOutScale, GameAllSet);
    }

    private void ReturnToMain()
    {
        SceneManager.LoadScene(0);
    }
}
