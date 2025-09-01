using System.Collections;
using UnityEngine;

public class FloatBounce : MonoBehaviour
{
    [SerializeField] private float floatDistance = 0.3f;
    [SerializeField] private float floatTime = 1.3f;
    [SerializeField] private float floatSpeed = 2f;

    private Vector3 _origin;
    private Coroutine _floatingEffect = null;

    private void OnEnable()
    {
        if (_floatingEffect != null)
        {
            StopCoroutine(_floatingEffect);
        }

        _origin = transform.position;

        _floatingEffect = StartCoroutine(CoFloatItem(
            transform.position + transform.up * floatDistance * 0.5f,
            transform.position - transform.up * floatDistance * 0.5f
            ));
    }

    private void OnDisable()
    {
        StopCoroutine(_floatingEffect);
    }

    private IEnumerator CoFloatItem(Vector3 startPos, Vector3 endPos)
    {
        transform.position = _origin;

        float elapsedTime = 0f;

        while (elapsedTime < floatTime)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, endPos, Time.deltaTime * floatSpeed);
            //transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / floatTime);
            yield return null;
        }

        _floatingEffect = StartCoroutine(CoFloatItem(endPos, startPos));
    }
}
