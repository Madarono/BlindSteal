using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[System.Serializable]
public class BloodSplashes
{
    public GameObject splash;
    public float requiredDmg;
}

public class BloodManager : MonoBehaviour
{
    public static BloodManager instance {get; private set;}
    
    public GameObject bloodParticle;

    [Header("Camera Shake FX")]
    public float amplitude = 10f;
    public float frequency = 3f;
    public float duration = 0.1f;
    public GameObject redPulse;
    public float redPulseDuration = 1f;
    public CinemachineImpulseSource impulseSource;
    public CinemachineImpulseListener impulseListener;

    [Header("Blood Splashes")]
    public BloodSplashes[] bloodSplashes;
    public float minScale = 0.1f;
    public float maxScale = 1f;
    public float minRot = -360f;
    public float maxRot = 360f;

    [Header("Death Splashes")]
    public GameObject[] dynamicSplashes;
    public int minSplashes = 2;
    public int maxSplashes = 4;
    public float minForce = 1f;
    public float maxForce = 4f;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        redPulse.SetActive(false);
    }

    public void MakeBloodParticle(Transform place)
    {
        GameObject blood = Instantiate(bloodParticle, place.position, Quaternion.Euler(-90, 0, 0));
        if(blood.TryGetComponent(out ParticleSystem bloodScript))
        {
            bloodScript.Play();
        }
        Destroy(blood, 0.7f);
    }

    public void MakeBloodSplash(Transform place, float damage)
    {
        List<BloodSplashes> splashes = new List<BloodSplashes>(bloodSplashes);
        for(int i = splashes.Count - 1; i >= 0; i--)
        {
            if(damage < splashes[i].requiredDmg)
            {
                splashes.RemoveAt(i);
            }
        }

        int randomSplash = Random.Range(0, splashes.Count);
        float randomScale = Random.Range(minScale, maxScale);
        float randomRot = Random.Range(minRot, maxRot);
        GameObject go = Instantiate(splashes[randomSplash].splash, place.position, Quaternion.Euler(0, 0, randomRot));
        go.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
    }

    public void DeathSplash(Transform place)
    {
        int splashAmount = Random.Range(minSplashes, maxSplashes);
        for(int i = 0; i < splashAmount; i++)
        {
            int randomSplash = Random.Range(0, dynamicSplashes.Length);
            float randomScale = Random.Range(minScale / 2, maxScale / 2);
            float randomRot = Random.Range(minRot, maxRot);
            GameObject go = Instantiate(dynamicSplashes[randomSplash], place.position, Quaternion.Euler(0, 0, randomRot));
            go.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
            if(go.TryGetComponent(out Rigidbody2D rb))
            {
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                float force = Random.Range(minForce, maxForce);
                rb.AddForce(randomDir * force, ForceMode2D.Impulse);
            }
        }
    }

    public void ScreenShake(float amplifyAmplitude, float amplifyFrequency, float amplifyDuration)
    {
        float finalDuration = duration * amplifyDuration;
        float finalAmplitude = amplitude * amplifyAmplitude;
        float finalFrequency = frequency * amplifyFrequency;

        int randomX = Random.Range(-1, 2);
        if(randomX == 0)
        {
            randomX = 1;
        }

        int randomY = Random.Range(-1, 2);
        if(randomY == 0)
        {
            randomY = 1;
        }
        impulseListener.m_ReactionSettings.m_AmplitudeGain = finalAmplitude;
        impulseListener.m_ReactionSettings.m_FrequencyGain = finalFrequency;
        impulseListener.m_ReactionSettings.m_Duration = finalDuration;
        impulseSource.m_ImpulseDefinition.m_ImpulseDuration = finalDuration;

        impulseSource.m_DefaultVelocity = new Vector3(randomX, randomY, 0);
        impulseSource.GenerateImpulse();
        // StartCoroutine(Pulse());
    }

    IEnumerator Pulse()
    {
        redPulse.SetActive(true);
        yield return new WaitForSeconds(redPulseDuration);
        redPulse.SetActive(false);
    }
}
