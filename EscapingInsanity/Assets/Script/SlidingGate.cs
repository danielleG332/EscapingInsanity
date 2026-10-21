using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SlidingGate : MonoBehaviour
{
    public float openHeight = 3.5f;
    public float openSpeed = 2f;

    private Vector3 closedPosition;
    private bool isOpen = false;
    private bool isMoving = false;

    void Start()
    {
        closedPosition = transform.position;
    }

    public void OpenGate()
    {
        if (!isOpen && !isMoving)
        {
            StartCoroutine(SlideOpen());
        }
    }

    private IEnumerator SlideOpen()
    {
        isMoving = true;

        Vector3 targetPosition =
            closedPosition + Vector3.up * openHeight;

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                openSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPosition;
        isOpen = true;
        isMoving = false;
    }
}
