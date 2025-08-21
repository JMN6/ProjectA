using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LightSensitivePlatform : MonoBehaviour, ILightSensitive
{
    [SerializeField] SpriteRenderer _sprite;
    [SerializeField] Collider2D _collider; //collider.enabled를 바꾸면 trigger가 다시 작동함..
    [SerializeField] bool _visibleInLight;

    CancellationTokenSource _tokenSource; 
    public void OnEnterLight()
    {
        try
        {
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
        }
        finally
        {
            _tokenSource = new();
        }

        SetActive(_visibleInLight, _tokenSource.Token).Forget();
    }

    public void OnExitLight()
    {
        try
        {
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
        }
        finally
        {
            _tokenSource = new();
        }

        SetActive(!_visibleInLight, _tokenSource.Token).Forget();
    }

    private async UniTaskVoid SetActive(bool active, CancellationToken token)
    {
        if (active)
        {
            await Transition(1f, 1f, _tokenSource.Token);
            _collider.isTrigger = false;
        }
        else
        {
            await Transition(0f, 1f, _tokenSource.Token);
            _collider.isTrigger = true;
        }
    }

    private async UniTask Transition(float targetAlpha, float transitionSpeed, CancellationToken token)
    {
        var color = _sprite.color;

        while (Mathf.Abs(targetAlpha - color.a) > Mathf.Epsilon)
        {
            color.a = Mathf.MoveTowards(color.a, targetAlpha, transitionSpeed * Time.deltaTime);

            _sprite.color = color;

            await UniTask.Yield(token);
        }

        color.a = targetAlpha;

        _sprite.color = color;
    }
}