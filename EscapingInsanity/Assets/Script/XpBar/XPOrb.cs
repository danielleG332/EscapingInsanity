using UnityEngine;

public class XPOrb : MonoBehaviour
{
    public int xpAmount = 25;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        XPManager xpManager =
            other.GetComponentInParent<XPManager>();

        if (xpManager != null)
        {
            collected = true;

            xpManager.AddXP(xpAmount);

            Destroy(gameObject);
        }
    }
}