using UnityEngine;

// ENCAPSULATION: all data is private.
// [SerializeField] lets us tune values in the Inspector without exposing them to other scripts.
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.2f;

    private CharacterController controller;
    private Vector3 velocity;            // only used for up/down speed (jumping and falling)

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
        if (Input.GetButtonDown("Jump")) Jump();
        ApplyGravity();
    }

    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Move relative to where the player faces; ClampMagnitude stops diagonal speed-up
        Vector3 direction = Vector3.ClampMagnitude(transform.right * x + transform.forward * z, 1f);

        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) speed *= sprintMultiplier;

        controller.Move(direction * speed * Time.deltaTime);   // deltaTime = same speed on any PC
    }

    // Public so other code may trigger a jump, but the rule (only when grounded) stays inside
    public void Jump()
    {
        if (controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);   // physics: v = sqrt(2 * g * h)
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;                                     // keeps the player pressed to the floor

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
