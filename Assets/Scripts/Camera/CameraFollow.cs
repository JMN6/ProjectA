using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform follow;

    [SerializeField] private Vector2 padding;

    public void SetFollow(Transform transform)
    {
        follow = transform;
    }

    private void LateUpdate()
    {
        if (follow != null)
        {
            var dx = transform.position.x - follow.position.x; // ����� ī�޶� ������
            var dy = transform.position.y - follow.position.y; // ����� ī�޶� ����

            if(dx < -padding.x)
            {
                transform.Translate(-padding.x - dx, 0, 0);
            }
            else if(dx > padding.x)
            {
                transform.Translate(padding.x - dx, 0, 0);
            }

            if (dy < -padding.y)
            {
                transform.Translate(0, -padding.y - dy, 0);
            }
            else if (dy > padding.y)
            {
                transform.Translate(0, padding.y - dy, 0);
            }

        }
        else
        {
            Debug.LogError("Follow is null!!!!!!!!!!!!!!!!!!!");
        }
    }
}
