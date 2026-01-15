using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    public GameObject light;
    private bool isOn = true;

    void Start()
    {
        isOn = true;
        light.SetActive(true);
    }

    void Update()   
    {
        if(Input.GetMouseButtonDown(1))
        {
            isOn = !isOn;
            light.SetActive(isOn);
        }
    }
}
