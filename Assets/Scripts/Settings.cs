using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings instance {get; private set;}
    public KeyCode sprint = KeyCode.LeftShift;
    public KeyCode interact = KeyCode.E;
    public KeyCode dash = KeyCode.Space;

    void Awake()
    {
        instance = this;
    }
}
