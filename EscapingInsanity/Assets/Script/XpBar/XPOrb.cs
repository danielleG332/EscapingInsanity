
using UnityEngine;

public class XPOrb : MonoBehaviour
{
    [Header("XP Settings")]
    public int xpAmount = 25;

    [Header("Magnet Settings")]
    public float attractionRange = 5f;
    public float attractionSpeed = 8f;
    public float collectionDistance = 0.5f;

    private Transform player;
    private bool collected = false;

    private void Start()
    {
        GameObject target = GameObject.FindGameObjectWithTag("Player");

        if (target != null)
            player = target.transform;
    }

    private void Update()
    {
        if (collected || player == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        // Start attracting the orb when the player is nearby.
        if (distance <= attractionRange)
        {
            Vector3 targetPosition = player.position + Vector3.up * 0.5f;

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                attractionSpeed * Time.deltaTime
            );
        }

        // Collect the orb when it reaches the player.
        if (distance <= collectionDistance)
        {
            CollectXP();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        XPManager xpManager =
            other.GetComponentInParent<XPManager>();

        if (xpManager != null)
        {
            CollectXP(xpManager);
        }
    }

    private void CollectXP(XPManager xpManager = null)
    {
        if (collected)
            return;

        collected = true;

        if (xpManager == null && player != null)
        {
            xpManager = player.GetComponentInParent<XPManager>();
        }

        if (xpManager != null)
        {
            xpManager.AddXP(xpAmount);
        }

        Destroy(gameObject);
    }
}
