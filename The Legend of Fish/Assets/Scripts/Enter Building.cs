using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnterBuilding : MonoBehaviour
{
    public UnityEvent enter;
    private void OnTriggerEnter(Collider other)
    {
        print(other.gameObject.CompareTag("Player"));
        if (other.gameObject.CompareTag("Player") && other.isTrigger) enter.Invoke();
    }
}
