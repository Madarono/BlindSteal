using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BarFillAmount
{
    public Sprite sprite;
    public float percentageNeeded;
}
public class BarSprites : MonoBehaviour
{
    public List<BarFillAmount> fillAmount = new List<BarFillAmount>();
    public Sprite[] sprites;
    public Image rend;

    void Start()
    {
        GenerateFillAmount();
    }

    void GenerateFillAmount()
    {
        fillAmount.Clear();

        for (int i = 0; i < sprites.Length; i++)
        {
            float percentage = 1 - ((float)i / (sprites.Length - 1));

            BarFillAmount fillAmountData = new BarFillAmount
            {
                sprite = sprites[i],
                percentageNeeded = percentage
            };

            fillAmount.Add(fillAmountData);
        }
    }

    public void UpdateBar(float inputPercentage)
    {
        inputPercentage = 1 - Mathf.Clamp01(inputPercentage);

        int index = Mathf.RoundToInt(inputPercentage * (sprites.Length - 1));

        rend.sprite = sprites[index];
    }
}
