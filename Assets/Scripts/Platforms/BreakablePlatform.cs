using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour, IStompSensitive, IResetable
{
    [SerializeField] float _allowTime;

    public void OnReset()
    {
        gameObject.SetActive(true);
    }

    // temp
    SpriteRenderer sr;
    public void OnStomped()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = Color.gray;
        InvokeBreak(_allowTime).Forget();
    }

    private async UniTaskVoid InvokeBreak(float time)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(time));

        gameObject.SetActive(false);
    }


}