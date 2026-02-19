using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Settings : MonoBehaviour
{
    public static Settings instance {get; private set;}
    public Transform player;
    public KeyCode sprint = KeyCode.LeftShift;
    public KeyCode interact = KeyCode.E;
    public KeyCode dash = KeyCode.Space;

    public Light2D[] totalLights;
    public Light2D globalLight;

    void Awake()
    {
        instance = this;
    }
}
