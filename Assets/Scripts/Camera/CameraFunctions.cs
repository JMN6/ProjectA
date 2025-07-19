using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraFunctions : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private float defaultSize;

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

        while(true)
        {
            elapsedTime += Time.deltaTime;
            float newScale = Mathf.Lerp(startScale, targetScale, elapsedTime / _time);
            cam.orthographicSize = newScale;

            if(elapsedTime >= _time)
            {
                cam.orthographicSize = newScale;
                break;
            }

            yield return null;
        }

        _callback?.Invoke();
    }


}
