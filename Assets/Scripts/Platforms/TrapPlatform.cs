using UnityEngine;

public class TrapPlatform : MonoBehaviour, IStompSensitive
{
    [SerializeField] float _damageAmount;
    [SerializeField] float _damageTerm;

    float _time;

    public void OnStomped()
    {
        if (Time.time - _time < _damageTerm) return;

        _time = Time.time;

        GameManager.Instance.Player.GetDamaged(1, gameObject);
    }
}