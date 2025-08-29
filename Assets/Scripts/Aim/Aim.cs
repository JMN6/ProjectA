using System;
using Game.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class Aim : MonoBehaviour
{
    public Gun CurGun { get; private set; }
    [SerializeField] private bool isFixed = true;
    [SerializeField] PlayerStatsSO _stat;
    [SerializeField] Light2D _flashLight;
    [SerializeField] CircleCollider2D _falshLightColiider;

    public int Ammo { get; private set; }
    public event Action onAimAmmoChanged;
    private void Awake()
    {
        CurGun = new Gun(1f, _stat);

        Ammo = _stat._initialAmmo;
        onAimAmmoChanged?.Invoke();
    }

    private void Start()
    {
        SetupInput();

        SetFlashLightRadius(5f);

        CurGun.Init();
    }

    private void SetupInput()
    {
        var input = InputManager.Instance;

        input.AddListener(Actions.Snipe, TriggerFlag.Performed, OnSnipe);
        input.AddListener(Actions.Shoot, TriggerFlag.Started, OnShoot);
        input.AddListener(Actions.Reload, TriggerFlag.Started, OnReload);
    }

    public void OnSnipe(InputAction.CallbackContext ctx)
    {
        var val = ctx.ReadValue<Vector2>();
        var sensitivity = GameManager.Instance.Option.MouseSensitivity;

        transform.Translate(val * sensitivity * sensitivity * 0.03f);
    }

    public void OnShoot(InputAction.CallbackContext ctx)
    {
        CurGun.Fire(transform.position);
    }

    public void OnReload(InputAction.CallbackContext ctx)
    {
        if (Ammo <= 0 || CurGun.LoadedBullets == Gun.MaxBullets)
        {
            SoundManager.Instance.PlaySFX(Path.SFX.Gun.SniperSFX.Empty);
            return;
        }

        ChangeAmmo(-1);
        CurGun.Reload();
    }

    public void ChangeAmmo(int amount)
    {
        Ammo += amount;
        onAimAmmoChanged?.Invoke();
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

    public void ScaleFlashLightRadius(float scale)
    {
        _flashLight.pointLightOuterRadius *= scale;
        _falshLightColiider.radius = _flashLight.pointLightOuterRadius;
    }

    public void SetFlashLightRadius(float radius)
    {
        _flashLight.pointLightOuterRadius = radius;
        _falshLightColiider.radius = _flashLight.pointLightOuterRadius;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out ILightSensitive obj))
        {
            obj = collision.GetComponentInParent<ILightSensitive>();
        }

        if (obj != null)
        {
            obj.OnEnterLight();
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out ILightSensitive obj))
        {
            obj = collision.GetComponentInParent<ILightSensitive>();
        }


        if (obj != null)
        {
            obj.OnExitLight();
        }
    }
}
