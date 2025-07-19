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

        // Dead zone ���� ���
        float leftBound = camPos.x - padding.x;
        float rightBound = camPos.x + padding.x;
        float bottomBound = camPos.y - padding.y;
        float topBound = camPos.y + padding.y;

        float newCamX = camPos.x;
        float newCamY = camPos.y;

        // �÷��̾ ������ �ٱ��� �ִ� ��츸 ī�޶� �̵�
        if (targetPos.x < leftBound) newCamX = targetPos.x + padding.x;
        else if (targetPos.x > rightBound) newCamX = targetPos.x - padding.x;

        if (targetPos.y < bottomBound) newCamY = targetPos.y + padding.y;
        else if (targetPos.y > topBound) newCamY = targetPos.y - padding.y;

        // �� ��� ���� (ī�޶� ũ�� ����)
        float clampedX = Mathf.Clamp(newCamX, bottom.position.x + camHalfWidth, top.position.x - camHalfWidth);
        float clampedY = Mathf.Clamp(newCamY, bottom.position.y + camHalfHeight, top.position.y - camHalfHeight);

        // ��ġ �ݿ�
        transform.position = new Vector3(clampedX, clampedY, camPos.z);
    }
}
