using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSubscription : MonoBehaviour
{
    public Vector2 MoveInput {  get; private set; } = Vector2.zero;
    public bool JumpInput { get; private set; } = false;
    public bool InteractInput { get; private set;} = false;
    public bool AttackInput { get; private set; } = false;
    public bool PauseInput { get; private set; } = false;
    public bool BlockInput { get; private set; } = false;

    InputSystem_Actions _Input = null;

    private void OnEnable()
    {
        _Input = new InputSystem_Actions();
        _Input.Player.Enable();

        _Input.Player.Move.performed += SetMovement;
        _Input.Player.Move.canceled += SetMovement;
        _Input.Player.Attack.started += Attack;
        _Input.Player.Attack.canceled += Attack;
        _Input.Player.Block.started += Block;
        _Input.Player.Block.canceled += Block;
    }
    private void OnDisable()
    {
        _Input.Player.Move.performed -= SetMovement;
        _Input.Player.Move.canceled -= SetMovement;
        _Input.Player.Attack.started -= Attack;
        _Input.Player.Attack.canceled -= Attack;
        _Input.Player.Block.started -= Block;
        _Input.Player.Block.canceled -= Block;
        _Input.Player.Disable();
    }
    void SetMovement(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }
    void Attack(InputAction.CallbackContext ctx)
    {
        AttackInput = ctx.started;
    }
    void Block(InputAction.CallbackContext ctx)
    {
        BlockInput = ctx.started;
    }
    private void Update()
    {
        JumpInput = _Input.Player.Jump.WasPressedThisFrame();
        InteractInput = _Input.Player.Interact.WasPressedThisFrame();
        PauseInput = _Input.Player.Pause.WasPressedThisFrame();
    }
}
