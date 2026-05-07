using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Mainscreenhandle : MonoBehaviour
{
    [SerializeField] InputSubscriptionUI input;
    bool started = false;
    public UnityEvent start = new UnityEvent();
    public UnityEvent unStart = new UnityEvent();
    [SerializeField] TMP_Text startText;
    [SerializeField] PlayerInput playerInput;
    string currentSchemeName;
    [SerializeField] Image buttonImage;
    //InputActionReference actionReference;
    //InputBinding defaultBinding = new InputBinding();
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        currentSchemeName = playerInput.currentControlScheme;
    }
    void Update()
    {
        currentSchemeName = playerInput.currentControlScheme;
        switch (currentSchemeName)
        {
            case "Gamepad":
            case "Joystick":
                startText.text = "Press ";
                buttonImage.gameObject.SetActive(true);
                break;
            case "Keyboard&Mouse":
                startText.text = "Press Space";
                buttonImage.gameObject.SetActive(false);
                break;
            default:
                startText.text = "Please use a different controller";
                buttonImage.gameObject.SetActive(false);
                break;
        }
        if (input.SubmitInput && !started)
        {
            start.Invoke();
            started = true;
            EventSystem.current.SetSelectedGameObject(FindFirstObjectByType<Button>().gameObject);
        }
        else if (input.BackInput && started)
        {
            unStart.Invoke();
            started = false;
        }
    }
}
