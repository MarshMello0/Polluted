using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [Header("Mouse Look Settings")]
    [SerializeField] private float sensitivity = 15F;
    [SerializeField] private  float minimumY = -60F;
    [SerializeField] private  float maximumY = 60F;
    private Transform _cameraTransform;
    private float rotationY = 0F;
    
    [Space]
    
    [Header("Movement Controls")] 
    [SerializeField] private KeyCode kForward;
    [SerializeField] private KeyCode kBackward, kRight, kLeft, kJump, kSprint;
    [SerializeField] private KeyCode kInventory; 
    
    [Space]
    
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float breakingSpeed;
    private float maxSpeed;
    [SerializeField] private float maxWalkingSpeed;
    [SerializeField] private float maxSprintSpeed;
    [SerializeField] private float gravityIncrease;
    [SerializeField] private float maxFallSpeed;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask floorMask;
    private Vector3 _inputs;
    private bool isGrounded;
    private Rigidbody _rigidbody;

    private bool inUI;
    private InventoryManager _inventoryManager;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _cameraTransform = transform.GetChild(0);
        _inventoryManager = GetComponent<InventoryManager>();
        maxSpeed = maxWalkingSpeed;
    }

    private void Update()
    {
        
        
        CameraMovement();
    }

    private void FixedUpdate()
    {
        Movement();

        if (inUI)
        {
            if (Input.GetKeyDown(kInventory) || Input.GetKeyDown(KeyCode.Escape))
            {
                _inventoryManager.CloseUI();
                inUI = false;
                LockCursor(true);
            }
        }
        else if (Input.GetKeyDown(kInventory) && !inUI)
        {
            _inventoryManager.OpenUI(InventoryManager.UIType.Inventory);
            inUI = true;
            LockCursor(false);
        }
        

    }

    private void Movement()
    {
        //Checking if the player is grounded
        isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, floorMask, QueryTriggerInteraction.Ignore);

        //Checking to see if they are sprinting
        if (Input.GetKeyDown(kSprint))
        {
            maxSpeed = maxSprintSpeed;
        }
        else if (Input.GetKeyUp(kSprint))
        {
            maxSpeed = maxWalkingSpeed;
        }
        
        //Checking for keys being pressed
        if (Input.GetKeyDown(kJump) && isGrounded)
        {
            _inputs.y = jumpHeight;
        }
        
        if (Input.GetKey(kForward))
        {
            _inputs.z += movementSpeed;    
        }
        else if (Input.GetKey(kBackward))
        {
            _inputs.z -= movementSpeed;
        }

        if (Input.GetKey(kLeft))
        {
            _inputs.x -= movementSpeed;
        }
        else if (Input.GetKey(kRight))
        {
            _inputs.x += movementSpeed;
        }
        
        //Gravity
        if (!isGrounded)
        {
            _inputs.y -= gravityIncrease;
        }

        //Clamping the max speed
        if (_inputs.x > maxSpeed)
            _inputs.x = maxSpeed;
        else if (_inputs.x < -maxSpeed)
            _inputs.x = -maxSpeed;
        if (_inputs.z > maxSpeed)
            _inputs.z = maxSpeed;
        else if (_inputs.z < -maxSpeed)
            _inputs.z = -maxSpeed;
        if (_inputs.y < -maxFallSpeed)
            _inputs.y = -maxFallSpeed;
        
        //Moving the player at a fixed frame rate
        _rigidbody.MovePosition(transform.TransformPoint(_inputs * Time.fixedDeltaTime));

        //Slowing down the player back to 0
        _inputs.x = Mathf.Lerp(_inputs.x, 0f, breakingSpeed * Time.fixedDeltaTime);
        _inputs.z = Mathf.Lerp(_inputs.z, 0f, breakingSpeed * Time.fixedDeltaTime);
        if (isGrounded && _inputs.y < 2.1f)
            _inputs.y = 0;
    }

    private void CameraMovement()
    {
        if (inUI)
            return;
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            LockCursor(true);
        }
        transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
        rotationY += Input.GetAxis("Mouse Y") * sensitivity;
        rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
        _cameraTransform.localEulerAngles = new Vector3(-rotationY, 0, 0);
    }
    
    public void LockCursor(bool state)
    {
        Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !state;
    }
}
