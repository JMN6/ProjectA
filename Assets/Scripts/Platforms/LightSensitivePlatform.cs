using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LightSensitivePlatform : MonoBehaviour, ILightSensitive
{
    [SerializeField] SpriteRenderer _sprite;
    [SerializeField] Animator _animator;
    [SerializeField] bool _visibleInLight;
    public void OnEnterLight()
    {
        SetActive(_visibleInLight);
    }

    public void OnExitLight()
    {
        SetActive(!_visibleInLight);
    }

    private void SetActive(bool active)
    {
        if (active)
        {
            _animator.ResetTrigger("Init");
            _animator.SetTrigger("Activate");
        }
        else
        {
            _animator.SetTrigger("Init");
        }
    }
}