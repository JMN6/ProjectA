using UnityEngine;
using UnityEngine.InputSystem;

public class Aim : MonoBehaviour
{
    [SerializeField] private Gun curGun;

    private SpriteRenderer sprite;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip shootSFX;

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
        var sensitivity = GameManager.Instance.Option.MouseSensitivity;

        transform.Translate(val * sensitivity * sensitivity * 0.01f);
    }

    public void OnShoot()
    {
        curGun.Shoot(transform.position);

        SoundManager.Instance.PlayOneShot(shootSFX);
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
