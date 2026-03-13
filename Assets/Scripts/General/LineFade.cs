using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineFade : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float fadeTime = 0.15f;

    private float timer = 0f;
    private Color startColor;

    void Awake()
    {
        startColor = lineRenderer.startColor;
    }

    public void ShowLine(Vector3 start, Vector3 end)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        timer = fadeTime;
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = startColor;
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            float alpha = timer / fadeTime;

            Color color = startColor;
            color.a = alpha;

            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }
}
