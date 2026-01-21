using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShadowVisibility : MonoBehaviour
{
    public SpriteRenderer spriteRend;

    [Header("Shadow Settings")]
    public LayerMask shadowLayer;
    public Light2D[] lights;
    public float totalDistance = 10f;

    [Header("Fade Settings")]
    public float fadeSpeed = 6f;

    float currentAlpha = 0f;

    void Start()
    {
        lights = Settings.instance.totalLights;
    }

    void Update()
    {
        float targetAlpha = 0f;

        foreach (var light in lights)
        {
            if(!light.enabled || !light.gameObject.activeInHierarchy)
            {
                continue;
            }

            Vector2 dir = (light.transform.position - transform.position).normalized;
            float dist = Vector2.Distance(light.transform.position, transform.position);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, shadowLayer);

            if (!hit)
            {
                float a = 1 - Mathf.Clamp01(dist / totalDistance);
                targetAlpha = Mathf.Max(targetAlpha, a);
            }
        }

        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);

        Color c = spriteRend.color;
        spriteRend.color = new Color(c.r, c.g, c.b, currentAlpha);
    }
}


