using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform follow;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector2 padding;
    private Rigidbody2D rigid;

    private Transform player;

    private Camera cam;
    [SerializeField] private Transform bottom;
    [SerializeField] private Transform top;

    [SerializeField] private CameraFunctions camFunc;

    [Header("OpenningSequence")]
    [SerializeField] private float showMapTime = 3f;
    [SerializeField] private float refocusToPlayer = 1.2f;
    public Vector3 startPos = new Vector3(-11.5f, 56.9f, -10f);
    public Vector3 endPos = new Vector3(-11.5f, 13.8f, -10f);
    [SerializeField] private Aim aim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
    }

    private int currentSequence = 0;
    public void OpenningSequence(Transform _player)
    {
        InputManager.Instance.HideCursor();
        InputManager.Instance.SetInputs(false);

        camFunc.Zoom(0f, 2.2f);
        SetFollow(null);

        player = _player;
        StartCoroutine(CoShowSequence());
    }

    //private void CameraSetting(SequenceInfo _nextInfo)
    //{
    //    ++currentSequence;
    //    aim.Fixed(_nextInfo.TargetObj, _nextInfo.Offset);
    //    camFunc.Zoom(_nextInfo.ScaleTime, _nextInfo.Scale,
    //        () =>
    //        {
    //            StartCoroutine(CoShowSequence(_nextInfo));
    //            aim.gameObject.SetActive(_nextInfo.ShowAim);
    //        });
    //}

    private IEnumerator CoShowSequence()
    {
        float elapsed = 0f;

        while(true)
        {
            elapsed += Time.deltaTime;

            if (elapsed >= showMapTime)
            {
                transform.position = endPos;
                break;
            }

            transform.position = Vector3.Lerp(startPos, endPos, elapsed / showMapTime);
            yield return null;
        }

        SetFollow(player);
        camFunc.Zoom(refocusToPlayer, 1, () => SequenceOver());
    }

    private void SequenceOver()
    {
        InputManager.Instance.SetInputs(true);
        aim.Fixed(player, Vector3.up * 1.2f);
        aim.gameObject.SetActive(true);
    }

    public void SetFollow(Transform transform)
    {
        follow = transform;
    }

    public void SetPadding(Vector2 _padding)
    {
        padding = _padding;
    }

    private void LateUpdate()
    {
        if (follow == null) return;

        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth = camHalfHeight * cam.aspect;

        Vector3 camPos = transform.position;
        Vector2 targetPos = follow.position;

        // Dead zone 범위 계산
        float leftBound = camPos.x - padding.x;
        float rightBound = camPos.x + padding.x;
        float bottomBound = camPos.y - padding.y;
        float topBound = camPos.y + padding.y;

        float newCamX = camPos.x;
        float newCamY = camPos.y;

        // 플레이어가 데드존 바깥에 있는 경우만 카메라 이동
        if (targetPos.x < leftBound) newCamX = targetPos.x + padding.x;
        else if (targetPos.x > rightBound) newCamX = targetPos.x - padding.x;

        if (targetPos.y < bottomBound) newCamY = targetPos.y + padding.y;
        else if (targetPos.y > topBound) newCamY = targetPos.y - padding.y;

        // 맵 경계 제한 (카메라 크기 포함)
        float clampedX = Mathf.Clamp(newCamX, bottom.position.x + camHalfWidth, top.position.x - camHalfWidth);
        float clampedY = Mathf.Clamp(newCamY, bottom.position.y + camHalfHeight, top.position.y - camHalfHeight);

        // 위치 반영
        transform.position = new Vector3(clampedX, clampedY, camPos.z);
    }
}
