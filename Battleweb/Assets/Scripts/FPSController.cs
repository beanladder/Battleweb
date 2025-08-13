using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float crouchSpeed = 3f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    
    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public Camera playerCamera;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    
    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchTransitionSpeed = 10f;
    
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;
    private float xRotation = 0f;
    private float currentSpeed;
    
    // Input variables
    private float horizontal;
    private float vertical;
    private bool isRunning;
    private bool jumpPressed;
    private bool crouchPressed;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        
        // Auto-assign if not set
        if (playerBody == null)
            playerBody = transform;
        if (playerCamera == null)
            playerCamera = Camera.main;
        
        // Set initial height
        controller.height = standingHeight;
    }
    
    void Update()
    {
        HandleInput();
        HandleMouseLook();
        HandleMovement();
        HandleCrouch();
        HandleJump();
        ApplyGravity();
    }
    
    void HandleInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        isRunning = Input.GetKey(KeyCode.LeftShift);
        jumpPressed = Input.GetButtonDown("Jump");
        crouchPressed = Input.GetKey(KeyCode.LeftControl);
    }
    
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        // Rotate camera up/down
        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // Rotate player body left/right
        if (playerBody != null)
            playerBody.Rotate(Vector3.up * mouseX);
        else
            transform.Rotate(Vector3.up * mouseX); // Fallback to self if no playerBody assigned
    }
    
    void HandleMovement()
    {
        // Determine current speed based on state
        if (isCrouching)
            currentSpeed = crouchSpeed;
        else if (isRunning && isGrounded)
            currentSpeed = runSpeed;
        else
            currentSpeed = walkSpeed;
        
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * currentSpeed * Time.deltaTime);
    }
    
    void HandleCrouch()
    {
        if (crouchPressed && !isCrouching)
        {
            isCrouching = true;
        }
        else if (!crouchPressed && isCrouching)
        {
            // Check if there's space to stand up
            if (!Physics.Raycast(transform.position, Vector3.up, standingHeight))
            {
                isCrouching = false;
            }
        }
        
        // Smooth height transition
        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        
        // Adjust camera position
        Vector3 cameraPos = playerCamera.transform.localPosition;
        float targetCameraY = isCrouching ? 0.5f : 1.6f;
        cameraPos.y = Mathf.Lerp(cameraPos.y, targetCameraY, Time.deltaTime * crouchTransitionSpeed);
        playerCamera.transform.localPosition = cameraPos;
    }
    
    void HandleJump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        if (jumpPressed && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    
    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    
    // Public getters for other scripts
    public bool IsMoving => horizontal != 0 || vertical != 0;
    public bool IsRunning => isRunning && IsMoving && isGrounded;
    public bool IsCrouching => isCrouching;
    public bool IsGrounded => isGrounded;
    public Vector2 MovementInput => new Vector2(horizontal, vertical);
}