using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFade : MonoBehaviour
{
    public SpriteRenderer rend;
    public float targetAlpha;
    public float speedOfTransition = 4f;

    public int objectsEntered;

    [Header("When Hovered")]
    public float hoverAlpha = 0.1f;

    void Start()
    {
        targetAlpha = 1f;
    }

    void Update()
    {
        Color color = rend.color;
        Color newColor = color;
        newColor = new Color(color.r, color.g, color.b, targetAlpha);


        color.a = Mathf.Lerp(color.a, newColor.a, Time.deltaTime * speedOfTransition);
        rend.color = color;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        targetAlpha = hoverAlpha;
        objectsEntered++;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        objectsEntered--;
        if(objectsEntered <= 0)
        {
            objectsEntered = 0;
            targetAlpha = 1f;
        }
    }
}
