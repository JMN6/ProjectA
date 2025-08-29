using System.Collections;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(LightExposure))]
public class EyeZombieVisibillity : MonoBehaviour
{
    [SerializeField] private float fadeTime = 0.1f;
    [SerializeField, Range(0f, 1f)] private float inShadowPercentage = 0.15f;
    [SerializeField, Range(0f, 1f)] private float inShadowAlpha = 1f;

    [SerializeField, Range(0f, 1f)] private float inLightPercentage = 0.25f;
    [SerializeField, Range(0f, 1f)] private float inLightAlpha = 0.21f;

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private LightExposure exposure;

    private Coroutine currentFade;
    private bool isInShadow;
    private bool isFading;

    private void Awake()
    {
        isFading = false;
        isInShadow = true;
        SetAlpha(inShadowAlpha);
    }

    private void Update()
    {
        if (isInShadow == true && exposure.LitPercentage >= inLightPercentage)
        {
            isInShadow = false;
            FadeToChangeAlpha(inLightAlpha);

        }
        else if (isInShadow == false && exposure.LitPercentage <= inShadowPercentage)
        {
            isInShadow = true;
            FadeToChangeAlpha(inShadowAlpha);
        }
    }

    private void FadeToChangeAlpha(float targetAlpha)
    {
        if (isFading == true)
            StopCoroutine(currentFade);

        isFading = true;
        currentFade = StartCoroutine(CoChangeAlpha(targetAlpha));
    }

    private IEnumerator CoChangeAlpha(float targetAlpha)
    {
        float elapsedTime = 0f;
        float currentAlpha = spriteRenderer.color.a;

        while(elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(currentAlpha, targetAlpha, elapsedTime / fadeTime);
            SetAlpha(newAlpha);
            yield return null;
        }
        SetAlpha(targetAlpha);
        isFading = false;
    }

    private void SetAlpha(float alpha)
    {
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }
}
