using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoSingleton<InputManager>
{
    [SerializeField] private List<PlayerInput> inputs = new List<PlayerInput>();

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
        foreach(var _ in inputs)
        {
            _.enabled = _val;
        }
    }
}
