using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Mainscreenhandle : MonoBehaviour
{
    [SerializeField] InputSubscriptionUI input;
    bool started = false;
    public UnityEvent start = new UnityEvent();
    public UnityEvent unStart = new UnityEvent();
    void Update()
    {
        if (input.SubmitInput && !started)
        {
            start.Invoke();
            started = true;
            EventSystem.current.SetSelectedGameObject(FindFirstObjectByType<Button>().gameObject);
        } else if (input.BackInput && started)
        {
            unStart.Invoke();
            started = false;
        }
    }
}
