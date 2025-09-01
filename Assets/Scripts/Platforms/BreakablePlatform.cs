using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour, IStompSensitive, IResetable
{
    [SerializeField] float _allowTime;
    [SerializeField] SpriteRenderer _sprite;
    [SerializeField] Sprite _originalSprite;
    [SerializeField] Sprite _brokenSprite;
    [SerializeField] Collider2D _collider;

    public void OnReset()
    {
        _sprite.sprite = _originalSprite;
        _collider.enabled = true;
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

        _sprite.sprite = _brokenSprite;
        _collider.enabled = false;
    }


}