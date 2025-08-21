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

        GameManager.Instance.Player.TryAttack(); // 패링해버릴 수도 있겠는디.. / 데미지 몇 줄 지도 받게 수정해야 할 듯
    }
}