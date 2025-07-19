using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoSingleton<InputManager>
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerInput aimInput;

    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetInputs(bool _val)
    {
        playerInput.enabled = _val;
        aimInput.enabled = _val;
    }

    public void SetPlayerInput(bool _val)
    {
        playerInput.enabled = _val;
    }

    public void SetAimInput(bool _val)
    {
        aimInput.enabled = _val;
    }
}
