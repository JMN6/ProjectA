using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour, IStompSensitive, IResetable
{
    [SerializeField] float _allowTime;
    [SerializeField] SpriteRenderer _sprite;
    [SerializeField] Animator _animator;
 
    public void OnReset()
    {
        _animator.SetTrigger("Init");
        _sprite.color = Color.white;
    }

    public void OnStomped()
    {
        InvokeBreak(_allowTime).Forget();
        _sprite.color = Color.gray;
    }

    private async UniTaskVoid InvokeBreak(float time)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(time));
        _animator.SetTrigger("Activate");
    }
}