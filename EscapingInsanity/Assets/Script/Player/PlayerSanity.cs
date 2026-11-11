using UnityEngine;


public class PlayerSanity : MonoBehaviour
{
    public float maxSanity = 100f;
    public float currentSanity;
    public float insanityChance = 0.5f;
    public float insanityTickDamage = 5f;
    public float insanityTickInvterval = 1f;
    private float tickTimer;

    public SanityBar sanityBar;
    public PlayerHealth controller;
    
    public Light playerLight;
    public float minLightRange = 5f;
    public float maxLightRange = 20f;
    private float targetLightRange;
    public float lightSmoothing = 5f;

    private bool isInsane;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (controller == null)
        {
            controller = GetComponent<PlayerHealth>();
        }

        currentSanity = maxSanity;

        if ( sanityBar != null)
        {
            sanityBar.SetMaxSanity(Mathf.RoundToInt(maxSanity));
        }

        targetLightRange = maxLightRange;

        if(playerLight != null)
        {
            playerLight.range = targetLightRange;
        }
    }

    public void LowerSanity(float damage)
    {
        if (damage <= 0f) return;

        if (Random.value <= insanityChance)
        {
            bool wasInsane = isInsane;
            currentSanity = Mathf.Max(0, currentSanity - damage);
            isInsane = currentSanity <= 0f;

            if (!wasInsane && isInsane)
            {
                tickTimer = insanityTickInvterval;
            }

            if(sanityBar != null)
            {
                sanityBar.SetSanity(Mathf.RoundToInt(currentSanity));
            }

            targetLightRange = Mathf.Lerp(minLightRange, maxLightRange, currentSanity / maxSanity);

        }

       
        
    }


    public void RestoreSanity(float amount)
    {
        if (amount <= 0f)
            return;

        currentSanity = Mathf.Clamp(
            currentSanity + amount,
            0f,
            maxSanity
        );

        // Update insanity state.
        isInsane = currentSanity <= 0f;

        // Reset the insanity damage timer if sanity is restored.
        if (!isInsane)
            tickTimer = insanityTickInvterval;

        // Update the sanity bar.
        if (sanityBar != null)
        {
            sanityBar.SetSanity(
                Mathf.RoundToInt(currentSanity)
            );
        }

        // Restore the player's light range.
        targetLightRange = Mathf.Lerp(
            minLightRange,
            maxLightRange,
            currentSanity / maxSanity
        );

        Debug.Log("Sanity restored! Current Sanity: " + currentSanity);
    }

    void Update()
    {
        if (playerLight != null)
        {
            playerLight.range = Mathf.Lerp(playerLight.range, targetLightRange, 1f - Mathf.Exp(-lightSmoothing * Time.deltaTime));
        }

        if(isInsane && controller != null)
        {
            tickTimer -= Time.deltaTime;

            if(tickTimer < 0f)
            {
                tickTimer = insanityTickInvterval;
                controller.TakeDamage(insanityTickDamage, false);
            }
        }
    }

    
}
