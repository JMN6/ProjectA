using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DataParseAndPlayer;
using System;

public class GameManager : MonoSingleton<GameManager>
{
    public Player Player { get; private set; }
    public Aim Aim { get; private set; }

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

    public int CurrentStage { get; private set; } = 0;
    public int Try { get; private set; } = 0;
    private bool _isGameCleared;

    private int maxStage;

    private float tempTimeScale = 1;

    public ItemsSO ItemsSO { get; private set; }

    public event Action<bool> onGameEnded;

    private void Awake()
    {
        ItemsSO = Resources.Load<ItemsSO>(Path.Data.Items);
        Player = Instantiate(Resources.Load<Player>(Path.Prefab.Player));
        Aim = Instantiate(Resources.Load<Aim>(Path.Prefab.Aim));
    }

    public void Start()
    {
        camFollow.OpenningSequence();
        maxStage = stageManager.StagetCount;

        Player.ResetPlayer(stageManager.GetRespawnPosition(CurrentStage),
            stageManager.GetRespawnFaceWay(CurrentStage));

        AnalyticsSystem.Instance.InitAsync().Forget();
    }

    private void OnApplicationQuit()
    {
        onGameEnded?.Invoke(_isGameCleared);
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
        CurrentStage = Mathf.Clamp(CurrentStage + 1, 0, maxStage);
        if (CurrentStage == maxStage)
        {
            GameClear();
            return;
        }
    }

    private void GameClear()
    {
        _isGameCleared = true;

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
        Player.ResetPlayer(stageManager.GetRespawnPosition(CurrentStage), 
            stageManager.GetRespawnFaceWay(CurrentStage) );

        stageManager.ResetGame(CurrentStage);

        camFunc.Zoom(zoomOutTime, zoomOutScale, GameAllSet);
    }

    private void ReturnToMain()
    {
        SceneManager.LoadScene(0);
    }
}
