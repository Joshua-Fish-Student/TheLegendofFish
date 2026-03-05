using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSubscriptionUI : MonoBehaviour
{
    public Vector2 MoveInput {  get; private set; } = Vector2.zero;
    public bool SubmitInput { get; private set;} = false;
    public bool BackInput { get; private set; } = false;
    public bool PauseInput { get; private set; } = false;

    InputSystem_Actions _Input = null;

    private void OnEnable()
    {
        _Input = new InputSystem_Actions();
        _Input.UI.Enable();

        _Input.UI.Navigate.performed += SetMovement;
        _Input.UI.Navigate.canceled += SetMovement;
    }
    private void OnDisable()
    {
        _Input.UI.Navigate.performed -= SetMovement;
        _Input.UI.Navigate.canceled -= SetMovement;
        _Input.UI.Disable();
    }
    void SetMovement(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }
    private void Update()
    {
        SubmitInput = _Input.UI.Submit.WasPressedThisFrame();
        BackInput = _Input.UI.Back.WasPressedThisFrame();
        PauseInput = _Input.UI.Pause.WasPressedThisFrame();
    }
}
