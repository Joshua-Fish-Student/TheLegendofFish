using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSubscriptionCamera : MonoBehaviour
{
    public Vector2 MoveInput {  get; private set; } = Vector2.zero;
    public bool ShiftInput { get; private set;} = false;

    public InputSystem_Actions _Input = null;

    private void OnEnable()
    {
        _Input = new InputSystem_Actions();
        _Input.CameraControl.Enable();

        _Input.CameraControl.PanCamera.performed += SetMovement;
        _Input.CameraControl.PanCamera.canceled += SetMovement;
        _Input.CameraControl.PanCameraKeyboard.performed += SetMovementKeyboard;
        _Input.CameraControl.PanCameraKeyboard.canceled += SetMovementKeyboard;
        _Input.CameraControl.SwitchtoCamerapankeyboard.started += Shift;
        _Input.CameraControl.SwitchtoCamerapankeyboard.canceled += Shift;
    }
    private void OnDisable()
    {
        _Input.CameraControl.PanCamera.performed -= SetMovement;
        _Input.CameraControl.PanCamera.canceled -= SetMovement;
        _Input.CameraControl.PanCameraKeyboard.performed -= SetMovementKeyboard;
        _Input.CameraControl.PanCameraKeyboard.canceled -= SetMovementKeyboard;
        _Input.CameraControl.SwitchtoCamerapankeyboard.started -= Shift;
        _Input.CameraControl.SwitchtoCamerapankeyboard.canceled -= Shift;
        _Input.CameraControl.Disable();
    }
    void SetMovement(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }
    void SetMovementKeyboard(InputAction.CallbackContext ctx)
    {
        if(ShiftInput)MoveInput = ctx.ReadValue<Vector2>();
    }
    void Shift(InputAction.CallbackContext ctx)
    {
        ShiftInput = ctx.started;
    }
}
