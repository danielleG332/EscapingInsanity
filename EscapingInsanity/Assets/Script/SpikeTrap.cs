
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public Transform[] spikes;
    public float raiseHeight = 0.5f;
    public float raiseDuration = 0.3f;

    private Vector3[] hiddenPositions;
    private bool activated = false;

    void Start()
    {
        hiddenPositions = new Vector3[spikes.Length];

        for (int i = 0; i < spikes.Length; i++)
        {
            hiddenPositions[i] = spikes[i].localPosition;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;

        if (other.CompareTag("Player"))
        {
            activated = true;
            StartCoroutine(RaiseSpikes());
        }
    }

    IEnumerator RaiseSpikes()
    {
        Vector3[] startPositions = new Vector3[spikes.Length];
        Vector3[] targetPositions = new Vector3[spikes.Length];

        for (int i = 0; i < spikes.Length; i++)
        {
            startPositions[i] = spikes[i].localPosition;
            targetPositions[i] = hiddenPositions[i]
                + Vector3.up * raiseHeight;
        }

        float elapsed = 0f;

        while (elapsed < raiseDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / raiseDuration);

            for (int i = 0; i < spikes.Length; i++)
            {
                spikes[i].localPosition = Vector3.Lerp(
                    startPositions[i],
                    targetPositions[i],
                    t
                );
            }

            yield return null;
        }

        for (int i = 0; i < spikes.Length; i++)
        {
            spikes[i].localPosition = targetPositions[i];
        }
    }
}
