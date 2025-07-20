using System.Collections;
using UnityEngine;
using System;

public class CameraFunctions : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private float defaultSize;
    [SerializeField] private Transform rain;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        defaultSize = cam.orthographicSize;
    }

    private Action callback;
    public void Zoom(float _time, float _scale, Action _callback = null)
    {
        StartCoroutine(CoZoom(_time, _scale, _callback));
    }

    private IEnumerator CoZoom(float _time, float _scale, Action _callback = null)
    {
        float startScale = cam.orthographicSize;
        float targetScale = defaultSize * _scale;
        float elapsedTime = 0f;

        float startRainScale = rain.localScale.x;
        float targetRainScale = _scale;

        while(true)
        {
            elapsedTime += Time.deltaTime;
            float newScale = Mathf.Lerp(startScale, targetScale, elapsedTime / _time);
            cam.orthographicSize = newScale;
            rain.transform.localScale = Vector3.one * 
                Mathf.Lerp(startRainScale, targetRainScale, elapsedTime / _time);

            if (elapsedTime >= _time)
            {
                cam.orthographicSize = newScale;
                break;
            }

            yield return null;
        }

        _callback?.Invoke();
    }

    public void Shake()
    {
        Zoom(0.08f, 1.05f,
            () => Zoom(0.05f, 0.9f,
            () => Zoom(0.05f, 1f)));
    }
}
