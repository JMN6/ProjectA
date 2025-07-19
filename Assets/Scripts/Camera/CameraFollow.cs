using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform follow;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector2 padding;
    private Rigidbody2D rigid;

    private Camera cam;
    [SerializeField] private Transform bottom;
    [SerializeField] private Transform top;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        cam = GetComponent<Camera>();
    }

    public void SetFollow(Transform transform)
    {
        follow = transform;
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
