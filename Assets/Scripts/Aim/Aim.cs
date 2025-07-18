using UnityEngine;
using UnityEngine.InputSystem;

public class Aim : MonoBehaviour
{
    [Range(0, 99)]
    [SerializeField] private float sensitivity;

    [SerializeField] private Gun curGun;

    public void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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


}
