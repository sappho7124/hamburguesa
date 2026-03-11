using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed = 720f;

    private CharacterController controller;
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector3 velocity;
    private bool isCrouching;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        
        controls.Player.Jump.performed += _ => Jump();
        controls.Player.Crouch.performed += _ => ToggleCrouch();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        // Gravity
        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;

        // Movement
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0; right.y = 0;
        Vector3 moveDir = (forward * moveInput.y + right * moveInput.x).normalized;

        float currentSpeed = isCrouching ? crouchSpeed : walkSpeed;
        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        // Rotation
        if (moveDir.magnitude > 0.1f)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveDir), rotationSpeed * Time.deltaTime);

        // Apply Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Jump()
    {
        if (controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void ToggleCrouch()
    {
        isCrouching = !isCrouching;
        controller.height = isCrouching ? 1f : 2f; // Shrink collider
    }
}