using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateText : MonoBehaviour
{

    float parentYRotation;
    void Update()
    {
        parentYRotation = transform.parent.rotation.y;
        transform.rotation = Quaternion.Euler(0f, -parentYRotation, 0f);
    }
}
