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
    public KeyCode building = KeyCode.B;

    public Light2D[] totalLights;
    public Light2D globalLight;
    public List<ShadowVisibility> visibles = new List<ShadowVisibility>();

    void Awake()
    {
        instance = this;
    }

    void RefreshVisibles()
    {
        for(int i = visibles.Count; i >= 0; i--)
        {
            if(visibles[i] == null)
            {
                visibles.RemoveAt(i);
                continue;
            }
            visibles[i].lights = totalLights;
        }
    }
}
