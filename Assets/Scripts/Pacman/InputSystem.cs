using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour
{
    public static InputSystem Instance { get; private set; }

    private PlayerInputActions _playerInputActions;
    private PlayerInputActions _resetMap;

    public event EventHandler OnResetButton;

    private void Awake()
    {
        Instance = this;

        _playerInputActions = new PlayerInputActions();
        _resetMap = new PlayerInputActions();
        _resetMap.Enable();
        _playerInputActions.Enable();
        _resetMap.Reset.ResetKey.started += ResetKey_started;
    }

    private void ResetKey_started(InputAction.CallbackContext obj)
    {
        OnResetButton?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        CleanupInputActions();
    }

    private void OnApplicationQuit()
    {
        CleanupInputActions();
    }

    private void CleanupInputActions()
    {
        if (_playerInputActions != null)
        {
            _playerInputActions.Disable();
            _playerInputActions.Dispose();
            _playerInputActions = null;
        }

        if (_resetMap != null)
        {
            _resetMap.Disable();
            _resetMap.Dispose();
            _resetMap = null;
        }
    }

    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();
        return inputVector;
    }
}