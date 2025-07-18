using UnityEngine;
using UnityEngine.InputSystem;

public class Aim : MonoBehaviour
{
    [Range(0, 99)]
    [SerializeField] private float sensitivity;

    [SerializeField] private Gun curGun;

    private SpriteRenderer sprite;

    public void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Show();

        curGun = new Sniper();
    }

    public void OnSnipe(InputValue input)
    {
        var val = input.Get<Vector2>();

        transform.Translate(val / (200 - sensitivity));
    }

    public void OnShoot()
    {
        curGun.Shoot(transform.position);
    }

    public void OnRecharge()
    {
        curGun.Recharge();
    }

    public void Hide()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        sprite.enabled = false;
    }

    public void Show()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        sprite.enabled = true;
    }
}
