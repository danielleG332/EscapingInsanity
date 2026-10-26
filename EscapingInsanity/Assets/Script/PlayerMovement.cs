using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float gravity = -9.81f;

    [Header("Camera Direction")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Animator characterAnimator;

    private Vector3 velocity;

    // 0 = North, 1 = East, 2 = South, 3 = West
    private int currentDirection = 0;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        characterAnimator = GetComponentInChildren<Animator>();

        // Automatically find the Main Camera if not assigned.
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        // WASD movement
        float x = 0f;
        float z = 0f;

        if (Keyboard.current.wKey.isPressed)
            z = 1f;

        if (Keyboard.current.sKey.isPressed)
            z = -1f;

        if (Keyboard.current.aKey.isPressed)
            x = -1f;

        if (Keyboard.current.dKey.isPressed)
            x = 1f;


        // Move relative to the camera's direction.
        Vector3 move;

        if (cameraTransform != null)
        {
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;

            // Ignore the camera's vertical angle.
            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            move = cameraForward * z + cameraRight * x;
        }
        else
        {
            // Fallback if no camera is assigned.
            move = transform.forward * z + transform.right * x;
        }

        // Sprint
        bool isSprinting = Keyboard.current.leftShiftKey.isPressed;

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // Move character
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Animation speed
        float speed = new Vector2(x, z).magnitude;
        // Blend Tree movement direction
        characterAnimator.SetFloat("MoveX", x);
        characterAnimator.SetFloat("MoveY", z);

        if (isSprinting && speed > 0)
        {
            characterAnimator.SetFloat("Speed", 2f);
        }
        else
        {
            characterAnimator.SetFloat("Speed", speed);
        }

        // Keep character on the ground
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        // Camera direction controls
        if (keyboard != null && cameraTransform != null)
        {
            int newDirection = currentDirection;

            if (keyboard.upArrowKey.wasPressedThisFrame)
                newDirection = 0;       // North

            else if (keyboard.rightArrowKey.wasPressedThisFrame)
                newDirection = 1;       // East

            else if (keyboard.downArrowKey.wasPressedThisFrame)
                newDirection = 2;       // South

            else if (keyboard.leftArrowKey.wasPressedThisFrame)
                newDirection = 3;       // West

            if (newDirection != currentDirection)
            {
                int difference = newDirection - currentDirection;

                cameraTransform.RotateAround(
                    transform.position,
                    Vector3.up,
                    difference * 90f
                );

                currentDirection = newDirection;
            }
        }
    }
      
    // For interacting with the Xp and Item objects
    private void OnTriggerEnter(Collider other)
    {
        Vector3 originalPosition = transform.position;
        Vector3 offset = new Vector3(5, 0, 0);

        if (other.gameObject.CompareTag("XP"))
        {
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.CompareTag("Item"))
        {
            other.gameObject.SetActive(false);
        }

    }

    // Attempt to make interactions with the Traps and Enemies.
    /*private void OnCollisionEnter(Collision collision)
    {
        Vector3 originalPosition = transform.position;
        Vector3 offset = new Vector3(5, 0, 0);

        if (collision.gameObject.CompareTag("Trap"))
        {
            Destroy(controller);
            Instantiate(controller);
            controller.Move(originalPosition + offset);

        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(controller);
            Instantiate(controller);
            controller.Move(originalPosition + offset);
            

        }
    }*/
}