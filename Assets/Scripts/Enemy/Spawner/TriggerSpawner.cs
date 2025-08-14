using UnityEngine;

public class TriggerSpawner : MonoBehaviour, IResetable
{
    [SerializeField] private Transform _trigger;
    [SerializeField] private SpawnPoint[] _points;

    public void OnReset()
    {
        _trigger.gameObject.SetActive(true);

        foreach (var point in _points)
        {
            point.OnReset();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _trigger.gameObject.SetActive(false);

        foreach (var point in _points)
        {
            point.Spawn();
        }
    }
}
