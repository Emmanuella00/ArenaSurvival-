using UnityEngine;

// ENCAPSULATION: all data is private. 
// Serialized fields help us to change values in the inspector without exposing them to other scripts. 
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.2f;

    private CharacterController controller;
    private Vector3 velocity;            

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

        Vector3 direction = Vector3.ClampMagnitude(transform.right * x + transform.forward * z, 1f);

        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) speed *= sprintMultiplier;      

        controller.Move(direction * speed * Time.deltaTime);                
    }

    public void Jump()
    {
        if (controller.isGrounded)                                      
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;                                     

        velocity.y += gravity * Time.deltaTime;                      
        controller.Move(velocity * Time.deltaTime);                   
    }
}
