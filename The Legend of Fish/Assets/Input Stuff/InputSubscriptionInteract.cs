using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSubscriptionInteract : MonoBehaviour
{
    public bool SubmitInput { get; private set; } = false;
    public bool PauseInput { get; private set; } = false;

    InputSystem_Actions _Input = null;

    private void OnEnable()
    {
        _Input = new InputSystem_Actions();
        _Input.Interacting.Enable();
    }
    private void OnDisable()
    {
        _Input.Interacting.Disable();
    }
    private void Update()
    {
        SubmitInput = _Input.Interacting.Submit.WasPressedThisFrame();
        PauseInput = _Input.Interacting.Pause.WasPressedThisFrame();
        _Input.Interacting.Pause.WasPerformedThisFrame();
    }
}
