using System;
using UnityEngine;
public class SkyboxSwitch : MonoBehaviour
{
    [SerializeField] Material day;
    [SerializeField] Material night;
    void Start()
    {
        if (DateTime.Now.Hour < 6 || DateTime.Now.Hour >= 18) RenderSettings.skybox = night;
        else RenderSettings.skybox = day;
    }
}