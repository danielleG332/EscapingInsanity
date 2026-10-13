using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;

    public float jumpHeight = 2f;
    public float gravity = -9.81f;


    private CharacterController controller;
    private Animator characterAnimator;

    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        characterAnimator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
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
            

        Vector3 move = transform.right * x + transform.forward * z;

        // Sprint
        bool isSprinting = Keyboard.current.leftShiftKey.isPressed;

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // Move character
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Animation speed
        float speed = new Vector2(x, z).magnitude;

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

        // Jump
if (Keyboard.current.spaceKey.wasPressedThisFrame)
{
    Debug.Log("SPACE PRESSED");

    Debug.Log("Grounded: " + controller.isGrounded);

    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

    characterAnimator.SetTrigger("JumpTrigger");
}

        // Gravity
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
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