using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sam Robichaud

public class S_FPS_Move : MonoBehaviour
{
    public bool canMove { get; private set; } = true;

    [Header("Keybinds")]
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] KeyCode jumpKey = KeyCode.Space;

    [Header("Movement Parameters")]
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float sprintSpeed = 8.5f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpHeight = 6.0f;
    [SerializeField] private float gravity = 15.0f;

    [Header("Look Parameters")]
    [SerializeField, Range(0.1f, 5)] private float mouseSensivityX = 1.5f;
    [SerializeField, Range(0.1f, 5)] private float mouseSensivityY = 1.5f;
    [SerializeField, Range(30, 90)] private float upperLookLimit = 70.0f;
    [SerializeField, Range(30, 90)] private float lowerLookLimit = 70.0f;
    [SerializeField] [Range(0.0f, 0.15f)] float mouseSmoothTime = 0.05f;

    private Transform playerCamera;
    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;
    float cameraPitch = 0.0f;


    private float antiBumpFactor = 0.75f;

    
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;


    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>().transform;
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void Update()
    {
        if (canMove)
        {
            HandleMoveInput();
            HandleMouseLook();
            HandleJump();

            ApplyFinalMovements();
        }
    }


    private void HandleMoveInput()
    {
        // Read inputs
        currentInput = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal") );        

        // normalizes input when 2 directions are pressed at the same time
        currentInput *= (currentInput.x != 0.0f && currentInput.y != 0.0f) ? 0.7071f : 1.0f;



        

    

        // this section checks for which input is pressed and adjust speed accordingly
        if (Input.GetKey(sprintKey) && currentInput.x == 1)
        {
            currentInput = currentInput * sprintSpeed;
        }
        else if (Input.GetKey(crouchKey) && characterController.isGrounded)
        {
            currentInput = currentInput * crouchSpeed;
        }
        else
        {
            currentInput = currentInput * walkSpeed;
        }

        

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }    

    private void HandleMouseLook()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        // player Look up/down
        cameraPitch -= currentMouseDelta.y * mouseSensivityX;
        cameraPitch = Mathf.Clamp(cameraPitch, -70.0f, 70.0f); // limits camera rotation up/down
        playerCamera.localEulerAngles = Vector3.right * cameraPitch;

        // player Look left/right     
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensivityX);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(jumpKey) && characterController.isGrounded )
        {            
            moveDirection.y = jumpHeight;
        }
    }


    private void ApplyFinalMovements()
    {
        // Apply gravity if the character controller is not grounded
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        // applies movement based on all inputs
        characterController.Move(moveDirection * Time.deltaTime);
    }

}
