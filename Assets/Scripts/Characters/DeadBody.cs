using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBody : MonoBehaviour
{
    public float timeTillFade = 1f;
    public SpriteRenderer rend;
    public float minForce = 1f;
    public float maxForce = 4f;

    private Coroutine currentCoroutine;

    public void Fade()
    {
        if(currentCoroutine != null)
        {
            return;
        }

        currentCoroutine = StartCoroutine(FadeAway());
    }
    
    IEnumerator FadeAway()
    {
        float t = 0;
        float alpha = 1;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float force = Random.Range(minForce, maxForce);
        rb.AddForce(randomDir * force, ForceMode2D.Impulse);

        while (t < timeTillFade)
        {
            t += Time.deltaTime;
            Color color = rend.color;
            alpha = Mathf.Lerp(1, 0, t / timeTillFade);
            rend.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}
