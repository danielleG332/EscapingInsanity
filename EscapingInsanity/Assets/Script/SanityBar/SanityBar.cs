using UnityEngine;
using UnityEngine.UI;

public class SanityBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetMaxSanity(int sanity)
    {
        slider.maxValue = sanity;
        slider.value = sanity;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetSanity(int sanity)
    {
        slider.value = sanity;
        fill.color = gradient.Evaluate(slider.normalizedValue);

    }
}
