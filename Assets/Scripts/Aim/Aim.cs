using UnityEngine;
using UnityEngine.InputSystem;

public class Aim : MonoBehaviour
{
    public Gun CurGun { get; private set; }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip shootSFX;
    [SerializeField] private AudioClip notEnoughBulletSFX;
    [SerializeField] private AudioClip reloadSFX;

    private CameraFunctions cameraFunctions;
    private float cooldown;

    private void Start()
    {
        Show();

        CurGun = new Sniper(1f, 5);

        cameraFunctions = Camera.main.GetComponent<CameraFunctions>();
    }

    private void Update()
    {
        if(cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }
    }

    public void OnSnipe(InputValue input)
    {
        var val = input.Get<Vector2>();
        var sensitivity = GameManager.Instance.Option.MouseSensitivity;

        transform.Translate(val * sensitivity * sensitivity * 0.03f);
    }

    public void OnShoot()
    {
        if (CurGun.CurBullets > 0)
        {
            if (cooldown <= 0.01f)
            {
                cooldown = CurGun.Cooltime;

                CurGun.Shoot(transform.position);
                cameraFunctions.Shake();
                SoundManager.Instance.PlayOneShot(shootSFX);
            }
        }
        else
        {
            SoundManager.Instance.PlaySFX(notEnoughBulletSFX);
        }
    }

    public void OnRecharge()
    {
        CurGun.Recharge();
        SoundManager.Instance.PlaySFX(reloadSFX);
    }

    public void Hide()
    {
        InputManager.Instance.ShowCursor();
    }

    public void Show()
    {
        InputManager.Instance.HideCursor();
    }

    private void LateUpdate()
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
