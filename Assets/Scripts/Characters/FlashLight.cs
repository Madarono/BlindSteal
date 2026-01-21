using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    public GameObject light;
    private bool isOn = true;
    public float checkDistance = 0.1f;
    public bool hittingWall = false;
    public LayerMask hittingLayers;

    void Start()
    {
        isOn = true;
        light.SetActive(true);
    }

    void Update()   
    {
        CheckWall();
        if(!hittingWall)
        {
            if(Input.GetMouseButtonDown(1))
            {
                isOn = !isOn;
            }
            
            light.SetActive(isOn);
        }
        else
        {
            light.SetActive(!hittingWall);
        }
    }

    void CheckWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, transform.right, checkDistance, hittingLayers);
        hittingWall = hit.collider ? true: false;

        Debug.DrawRay(gameObject.transform.position, transform.right * checkDistance, Color.red);
    }
}
