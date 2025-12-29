using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour
{
    public static InputSystem Instance { get; private set; }

    private PlayerInputActions _playerInputActions;
    private PlayerInputActions _resetMap;


    private void Awake()
    {
        Instance = this;

        _playerInputActions = new PlayerInputActions();
        _resetMap = new PlayerInputActions();
        _resetMap.Enable();
        _playerInputActions.Enable();

    }

    public float GetReset()
    {
        float reset = _resetMap.Reset.ResetKey.ReadValue<float>();
        return reset;
    }

    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();
        return inputVector;
    }

    //public void DisableMovement()
    //{
    //    _playerInputActions.Disable();
    //}



}