using Cysharp.Threading.Tasks;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class AnalyticsSystem : Singleton<AnalyticsSystem>
{
    [Header("Analytics Info")]
    [SerializeField] bool _isInitialized;
    [SerializeField] string _userId;
    [SerializeField] string _sessionId;
    [SerializeField] float _startTime;

    [Header("Settings")]
    [SerializeField] float _startDelay = 5f;
    [SerializeField] float _sendInterval = 10f;
    [SerializeField] int _loopCount = -1; // -1 for infinite

    public async UniTaskVoid InitAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();

            AnalyticsService.Instance.StartDataCollection();

            _startTime = Time.time;

            GameManager.Instance.onGameEnded += OnGameEnded;
            GameManager.Instance.Player.onPlayerHitBefore += OnPlayerHitBefore;
            RecordPlayerPosition(_loopCount).Forget();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to initialize Analytics Service: {e.Message}");
        }
    }

    private async UniTaskVoid RecordPlayerPosition(int loopCount)
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(_startDelay), DelayType.DeltaTime);

        var evt = new CustomEvent("player_position");

        for (int i = 0; i < loopCount || loopCount == -1; i++)
        {
            if (GameManager.Instance.Player != null)
            {
                evt["playTime"] = (Time.time - _startTime).ToString("F2");
                evt["playerX"] = GameManager.Instance.Player.transform.position.x.ToString("F2");
                evt["playerY"] = GameManager.Instance.Player.transform.position.y.ToString("F2");
                evt["stage"] = GameManager.Instance.CurrentStage;

                AnalyticsService.Instance.RecordEvent(evt);
            }

            await UniTask.Delay(System.TimeSpan.FromSeconds(_sendInterval), DelayType.DeltaTime);
        }
    }

    public void OnPlayerHitBefore(GameObject damageSource)
    {
        var player = GameManager.Instance.Player;

        var evt = new CustomEvent("player_hit")
        {
            { "playTime", (Time.time - _startTime).ToString("F2") },
            { "playerX", player.transform.position.x.ToString("F2") },
            { "playerY", player.transform.position.y.ToString("F2") },
            { "damage_source", damageSource.name },
            { "hp_before", player.CurHP },
        };

        AnalyticsService.Instance.RecordEvent(evt);
    }

    private void OnGameEnded(bool isGameCleared)
    {
        var evt = new CustomEvent("game_end")
        {
            { "playTime", (Time.time - _startTime).ToString("F2") },
            { "try", GameManager.Instance.Try },
            { "clear", isGameCleared },
        };

        AnalyticsService.Instance.RecordEvent(evt);
    }

    public void OnPlayerGotItem(int itemId)
    {
        var player = GameManager.Instance.Player;

        var evt = new CustomEvent("item_get")
        {
            { "playTime", (Time.time - _startTime).ToString("F2") },
            { "playerX", player.transform.position.x.ToString("F2") },
            { "playerY", player.transform.position.y.ToString("F2") },
            { "item_name", GameManager.Instance.ItemsSO.GetItem(itemId).Name },
        };

        AnalyticsService.Instance.RecordEvent(evt);
    }

    public void OnEnemyKilled(Enemy enemy, GameObject damageSource)
    {
        var player = GameManager.Instance.Player;

        var evt = new CustomEvent("enemy_kill")
        {
            { "playTime", (Time.time - _startTime).ToString("F2") },
            { "playerX", player.transform.position.x.ToString("F2") },
            { "playerY", player.transform.position.y.ToString("F2") },
            { "enemy_x", enemy.transform.position.x.ToString("F2") },
            { "enemy_y", enemy.transform.position.y.ToString("F2") },
            { "enemy_name", enemy.name },
            { "damage_source", damageSource.name },
        };

        AnalyticsService.Instance.RecordEvent(evt);
    }
}
