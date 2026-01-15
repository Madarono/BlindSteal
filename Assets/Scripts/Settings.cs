using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings instance {get; private set;}
    public KeyCode sprint = KeyCode.LeftShift;
    public KeyCode interact = KeyCode.E;

    void Awake()
    {
        instance = this;
    }
}
