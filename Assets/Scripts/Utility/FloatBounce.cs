using System.Collections;
using UnityEngine;

public class FloatBounce : MonoBehaviour
{
    [SerializeField] private float floatDistance = 0.3f;
    [SerializeField] private float floatTime = 1.3f;
    [SerializeField] private float floatSpeed = 2f;

    private void Start()
    {
        StartCoroutine(CoFloatItem(
            transform.position + transform.up * floatDistance * 0.5f,
            transform.position - transform.up * floatDistance * 0.5f
            ));
    }

    private IEnumerator CoFloatItem(Vector3 startPos, Vector3 endPos)
    {
        float elapsedTime = 0f;

        while (elapsedTime < floatTime)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, endPos, Time.deltaTime * floatSpeed);
            //transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / floatTime);
            yield return null;
        }

        StartCoroutine(CoFloatItem(endPos, startPos));
    }
}
