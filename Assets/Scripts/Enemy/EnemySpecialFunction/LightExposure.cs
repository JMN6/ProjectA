using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LightExposure : MonoBehaviour
{
    [SerializeField] private LayerMask lightAndZombieLayer;

    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material allWhiteMaterial;
     
    [Header("RT & Sampling")]
    [Range(32, 256)] private int rtSize = 128;
    [Range(0f, 0.6f)] private float boundMargin = 0.15f;
    public float sampleCheckOffset = 0.05f;

    [Range(0f, 1f)] private float lightMinimumValue = 0.35f;

    public float LitPercentage { get; private set; }
    private int totalPixelCount;
    private int litPixelCount;

    private Camera checkCam;
    private RenderTexture rt;
    private SpriteRenderer sr;
    private float elapsedTime = 0f;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        var g = new GameObject($"{name}_checkCam");
        checkCam = g.AddComponent<Camera>();
        checkCam.orthographic = true;
        checkCam.clearFlags = CameraClearFlags.SolidColor;
        checkCam.backgroundColor = Color.black;
        checkCam.cullingMask = lightAndZombieLayer;
        checkCam.allowHDR = true;
        checkCam.enabled = false;

        rt = new RenderTexture(rtSize, rtSize, 0, RenderTextureFormat.ARGB32)
        {
            name = $"{name}_CheckRT",
            filterMode = FilterMode.Point,
            useMipMap = false,
            autoGenerateMips = false
        };
        rt.Create();
        checkCam.targetTexture = rt;
    }

    private void OnDisable()
    {
        AsyncGPUReadback.WaitAllRequests();
    }

    private void OnDestroy()
    {
        AsyncGPUReadback.WaitAllRequests();

        if(rt != null)
            rt.Release();

        if(checkCam != null )
            Destroy(checkCam.gameObject);
    }

    private void LateUpdate()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime < sampleCheckOffset)
            return;

        elapsedTime = 0f;

        var b = sr.bounds;
        var ext = b.extents;

        float half = Mathf.Max(ext.y, ext.x / 1f) * (1f + boundMargin);
        checkCam.orthographicSize = Mathf.Max(half, 0.01f);

        var pos = b.center; pos.z = -10f;
        checkCam.transform.position = pos;

        sr.material = allWhiteMaterial;

        // 한 프레임 렌더
        checkCam.Render();

        sr.material = defaultMaterial;

        AsyncGPUReadback.Request(rt, 0, TextureFormat.RGBA32, OnRTRead);
    }

    private void OnGUI()
    {
        if (rt)
            GUI.DrawTexture(new Rect(10, 10, 256, 256), rt, ScaleMode.ScaleToFit, false);
    }

    private void OnRTRead(AsyncGPUReadbackRequest req)
    {
        if (req.hasError == true)
            return;

        var data = req.GetData<Color32>();

        int total = data.Length;
        totalPixelCount = litPixelCount = 0;

        for (int i = 0; i < total; i++)
        {
            var pixel = data[i];

            if (pixel.a <= 0)
                continue;

            ++totalPixelCount;
            float y = (0.2126f * pixel.r + 
                0.7152f * pixel.g + 
                0.0722f * pixel.b) / 255f;

            if(y >= lightMinimumValue)
                ++litPixelCount;
        }

        LitPercentage = totalPixelCount > 0 ? (float)litPixelCount / totalPixelCount : 0f;
        LitPercentage *= 10f;
    }
}
