using UnityEngine;
using UnityEngine.InputSystem;

public class Aim : MonoBehaviour
{
    public Gun CurGun { get; private set; }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip shootSFX;


    private void Start()
    {
        Show();

        CurGun = new Sniper();
    }

    public void OnSnipe(InputValue input)
    {
        var val = input.Get<Vector2>();
        var sensitivity = GameManager.Instance.Option.MouseSensitivity;

        transform.Translate(val * sensitivity * sensitivity * 0.01f);
    }

    public void OnShoot()
    {
        CurGun.Shoot(transform.position);

        // HAVETOMOVE
        SoundManager.Instance.PlayOneShot(shootSFX);
    }

    public void OnRecharge()
    {
        CurGun.Recharge();
    }

    public void Hide()
    {
        InputManager.Instance.ShowCursor();
    }

    public void Show()
    {
        InputManager.Instance.HideCursor();
    }

    private void Update()
    {
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
