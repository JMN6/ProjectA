using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Game.Input;

public class InputManager : MonoSingleton<InputManager>
{

    private static InputActions _input;

    private Dictionary<Maps, InputActionMap> _maps;
    private Dictionary<Actions, InputAction> _actions;

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerInput aimInput;

    private void Awake()
    {
        _input = new InputActions();
        _input.Enable();

        var maps = (Maps[])Enum.GetValues(typeof(Maps));
        _maps = new(maps.Length);

        foreach (var map in maps)
        {
            _maps[map] = _input.asset.FindActionMap(map.ToString());
        }

        var actions = (Actions[])Enum.GetValues(typeof(Actions));
        _actions = new(actions.Length);

        foreach (var action in actions)
        {
            _actions[action] = _input.FindAction(action.ToString());
        }
    }

    public void AddListener(Actions input, Action<InputAction.CallbackContext> callback)
    {
        if (_actions.TryGetValue(input, out InputAction action)) {
            action.started += callback;
            action.canceled += callback;
        }
    }

    public void RemoveListener(Actions input, Action<InputAction.CallbackContext> callback)
    {
        if (_actions.TryGetValue(input, out InputAction action))
        {
            action.started -= callback;
            action.canceled -= callback;
        }
    }

    public void LockCursor(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void SetInputEnabled(Maps map, bool enabled)
    {
        if (_maps.TryGetValue(map, out InputActionMap input))
        {
            if (enabled) input.Enable();
            else input.Disable();
        }
    }

    public void ShowCursor()
    {
        LockCursor(false);
    }

    public void HideCursor()
    {
        LockCursor(true);
    }

    public void SetInputs(bool _val)
    {
        SetInputEnabled(Maps.Player, _val);
        SetInputEnabled(Maps.Aim, _val);
    }

    public void SetPlayerInput(bool _val)
    {
        SetInputEnabled(Maps.Player, _val);
    }

    public void SetAimInput(bool _val)
    {
        SetInputEnabled(Maps.Aim, _val);
    }
}
