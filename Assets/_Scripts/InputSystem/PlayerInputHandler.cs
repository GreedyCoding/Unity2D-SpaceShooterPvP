using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MovementInput;

    public bool ShootInput;
    public bool DashInput;
    public bool EscapeInput;

    PlayerInput PlayerInputScheme;

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        PlayerInputScheme = new PlayerInput();
        PlayerInputScheme.DefaultMap.Enable();

        PlayerInputScheme.DefaultMap.Move.performed += SetMoveInput;
        PlayerInputScheme.DefaultMap.Move.canceled += SetMoveInput;

        PlayerInputScheme.DefaultMap.Shoot.performed += SetShootInput;
        PlayerInputScheme.DefaultMap.Shoot.canceled += ResetShootInput;

        PlayerInputScheme.DefaultMap.Dash.performed += SetDashInput;
        PlayerInputScheme.DefaultMap.Dash.canceled += ResetDashInput;

        PlayerInputScheme.DefaultMap.Escape.performed += SetEscapeInput;
        PlayerInputScheme.DefaultMap.Escape.canceled += ResetEscapeInput;
    }

    private void UnsubscribeFromEvents()
    {
        PlayerInputScheme.DefaultMap.Move.performed -= SetMoveInput;
        PlayerInputScheme.DefaultMap.Move.canceled -= SetMoveInput;

        PlayerInputScheme.DefaultMap.Shoot.performed -= SetShootInput;
        PlayerInputScheme.DefaultMap.Shoot.canceled -= ResetShootInput;

        PlayerInputScheme.DefaultMap.Dash.performed -= SetDashInput;
        PlayerInputScheme.DefaultMap.Dash.canceled -= ResetDashInput;

        PlayerInputScheme.DefaultMap.Escape.performed -= SetEscapeInput;
        PlayerInputScheme.DefaultMap.Escape.canceled -= ResetEscapeInput;

        PlayerInputScheme.DefaultMap.Disable();
    }

    private void SetMoveInput(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    private void SetShootInput(InputAction.CallbackContext context)
    {
        ShootInput = true;
    }
    private void ResetShootInput(InputAction.CallbackContext context)
    {
        ShootInput = false;
    }

    private void SetDashInput(InputAction.CallbackContext context)
    {
        DashInput = true;
    }
    private void ResetDashInput(InputAction.CallbackContext context)
    {
        DashInput = false;
    }

    private void SetEscapeInput(InputAction.CallbackContext context)
    {
        EscapeInput = true;
    }
    private void ResetEscapeInput(InputAction.CallbackContext context)
    {
        EscapeInput = false;
    }
}
