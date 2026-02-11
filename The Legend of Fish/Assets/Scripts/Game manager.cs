using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    InputSubscription GetInput;
    private void Awake()
    {
        GetInput = GetComponent<InputSubscription>();
    }
}
