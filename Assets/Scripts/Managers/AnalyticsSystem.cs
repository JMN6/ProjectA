using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AnalyticsSystem : Singleton<AnalyticsSystem>
{
    [Header("Analytics Info")]
    [SerializeField] float _startTime;

    [Header("Settings")]
    [SerializeField] float _startDelay = 5f;
    [SerializeField] float _sendInterval = 10f;
    [SerializeField] int _loopCount = -1; // -1 for infinite

    private List<AnalyticsEvent> _events = new List<AnalyticsEvent>();
    private string _savePath;

    [Serializable]
    public struct AnalyticsEvent
    {
        public string name;
        public float playTime;
        public List<KeyValue> data; // Dictionary ´ëÃ¼¿ë
    }

    [Serializable]
    public struct KeyValue
    {
        public string key;
        public string value;
    }

    public void InitAsync()
    {
        _startTime = Time.time;
        _savePath = System.IO.Path.Combine(Application.persistentDataPath, "analytics.json");

        GameManager.Instance.onGameEnded += OnGameEnded;
        GameManager.Instance.Player.onPlayerHitBefore += OnPlayerHitBefore;
        RecordPlayerPosition(_loopCount).Forget();
    }

    private async UniTaskVoid RecordPlayerPosition(int loopCount)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_startDelay), DelayType.DeltaTime);

        for (int i = 0; i < loopCount || loopCount == -1; i++)
        {
            if (GameManager.Instance.Player != null)
            {
                AddEvent("player_position", new Dictionary<string, object>
                {
                    { "playerX", GameManager.Instance.Player.transform.position.x },
                    { "playerY", GameManager.Instance.Player.transform.position.y },
                    { "stage", GameManager.Instance.CurrentStage }
                });
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_sendInterval), DelayType.DeltaTime);
        }
    }

    public void OnPlayerHitBefore(GameObject damageSource)
    {
        var player = GameManager.Instance.Player;
        AddEvent("player_hit", new Dictionary<string, object>
        {
            { "damage_source", damageSource.name },
            { "hp_before", player.CurHP },
            { "playerX", player.transform.position.x },
            { "playerY", player.transform.position.y }
        });
    }

    private void OnGameEnded(bool isGameCleared)
    {
        AddEvent("game_end", new Dictionary<string, object>
        {
            { "try", GameManager.Instance.Try },
            { "clear", isGameCleared }
        });

        SaveToJson();
    }

    public void OnPlayerGotItem(int itemId)
    {
        var player = GameManager.Instance.Player;
        if (player == null) return;

        AddEvent("item_get", new Dictionary<string, object>
        {
            { "item_name", GameManager.Instance.ItemsSO.GetItem(itemId).Name },
            { "playerX", player.transform.position.x },
            { "playerY", player.transform.position.y }
        });
    }

    public void OnEnemyKilled(Enemy enemy, GameObject damageSource)
    {
        var player = GameManager.Instance.Player;
        AddEvent("enemy_kill", new Dictionary<string, object>
        {
            { "enemy_x", enemy.transform.position.x },
            { "enemy_y", enemy.transform.position.y },
            { "enemy_name", enemy.name },
            { "damage_source", damageSource.name },
            { "playerX", player.transform.position.x },
            { "playerY", player.transform.position.y }
        });
    }

    private void AddEvent(string type, Dictionary<string, object> data)
    {
        var kvList = new List<KeyValue>();
        foreach (var kv in data)
        {
            kvList.Add(new KeyValue
            {
                key = kv.Key,
                value = kv.Value.ToString()
            });
        }

        _events.Add(new AnalyticsEvent
        {
            name = type,
            playTime = (Time.time - _startTime),
            data = kvList
        });
    }


    public void SaveToJson()
    {
        string json = JsonUtility.ToJson(new Wrapper { events = _events }, true);
        File.WriteAllText(_savePath, json);
        Debug.Log($"Analytics saved to: {_savePath}");
    }

    [Serializable]
    private class Wrapper
    {
        public List<AnalyticsEvent> events;
    }
}
