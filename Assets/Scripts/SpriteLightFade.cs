using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpriteLightFade : MonoBehaviour
{
    public Light2D flashlight;
    public SpriteRenderer sprite;
    public float minLightIntensity = 0.1f;
    public Light2D spriteLight;
    public float minIntensity = 0f;
    public float maxIntensity = 2.5f;

    void Update()
    {
        float distance = Vector2.Distance(flashlight.transform.position, transform.position);
        
        Color c = sprite.color;
        float alpha = Mathf.Clamp01(1 - (distance / flashlight.pointLightOuterRadius));
        sprite.color = new Color(c.r, c.g, c.b, alpha);

        float t = Mathf.Clamp01(distance / flashlight.pointLightOuterRadius);
        spriteLight.intensity = Mathf.Lerp(maxIntensity, minIntensity, t);

        if (distance > flashlight.pointLightOuterRadius || flashlight.intensity < minLightIntensity)
        {
            sprite.enabled = false;
        }
        else
        {
            sprite.enabled = true;
        }
    }
}

