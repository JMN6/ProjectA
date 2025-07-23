using UnityEngine;
using UnityEngine.InputSystem;

public class Aim : MonoBehaviour
{
    public Gun CurGun { get; private set; }
    [SerializeField] private bool isFixed = true;

    private void Start()
    {
        Show();

        CurGun = new Sniper(1f, 5);
    }

    public void OnSnipe(InputValue input)
    {
        var val = input.Get<Vector2>();
        var sensitivity = GameManager.Instance.Option.MouseSensitivity;

        transform.Translate(val * sensitivity * sensitivity * 0.03f);
    }

    public void OnShoot()
    {
        CurGun.Fire(transform.position);
    }

    public void OnReload()
    {
        CurGun.Reload();
    }

    public void Hide()
    {
        InputManager.Instance.ShowCursor();
    }

    public void Show()
    {
        InputManager.Instance.HideCursor();
    }

    private Transform fixedTarget;
    private Vector3 offset;
    public void Fixed(Transform _target, Vector3 _offset)
    {
        transform.position = _target.position + offset;
        return;
        isFixed = true;
        fixedTarget = _target;
        offset = _offset;
    }

    public void Free()
    {
        isFixed = false;
    }

    private void LateUpdate()
    {
        //if(isFixed)
        //{
        //    transform.position = fixedTarget.position + offset;

        //    return;
        //}

        float camHalfHeight = Camera.main.orthographicSize;
        float camHalfWidth = camHalfHeight * Camera.main.aspect;
        Vector3 camPos = Camera.main.transform.position;

        float minX = camPos.x - camHalfWidth;
        float maxX = camPos.x + camHalfWidth;
        float minY = camPos.y - camHalfHeight;
        float maxY = camPos.y + camHalfHeight;

        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
