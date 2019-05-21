using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.Rendering;
using Random = UnityEngine.Random;

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
    public KeyCode kForward;
    public KeyCode kBackward, kRight, kLeft, kJump, kSprint;
    [SerializeField] private KeyCode kInventory;
    [SerializeField] private KeyCode kScreenshot = KeyCode.F12;

    [Space] 
    
    [Header("Movement Settings")] 
    public bool canMove;
    public bool canJump;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float breakingSpeed;
    private float maxSpeed;
    [SerializeField] private float maxWalkingSpeed;
    [SerializeField] private float maxSprintSpeed;
    [SerializeField] private float gravityIncrease;
    [SerializeField] private float gravity;
    [SerializeField] private float maxFallSpeed;
    [SerializeField] private Transform groundChecker, headCheacker;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask floorMask;
    private Vector3 _inputs;
    private bool isGrounded;
    [SerializeField] private CharacterController characterController;

    [HideInInspector]
    public bool inUI;
    private InventoryManager _inventoryManager;

    [SerializeField] private GameObject miniMap;

    [HideInInspector]
    public bool spawnSet;
    private bool ready = true;
    private bool hitHead;

    private void Awake()
    {
        _cameraTransform = transform.GetChild(0);
        _inventoryManager = GetComponent<InventoryManager>();
        maxSpeed = maxWalkingSpeed;
        SetControls();
        Mouse.Lock(true);
    }
    private void Update()
    {
        CameraMovement();
        Movement();
        if (Input.GetKeyDown(kScreenshot))
        {
            StartCoroutine(TakeScreenShot());
        }
    }

    private void FixedUpdate()
    {
        Gravity();
        if (!spawnSet)
        {
            MoveToSpawn();
            spawnSet = true;
        }
        if (inUI)
        {
            if (Input.GetKeyDown(kInventory) || Input.GetKeyDown(KeyCode.Escape))
            {
                if (ready)
                {
                    _inventoryManager.CloseUI();
                    inUI = false;
                    Mouse.Lock(true);
                    StartCoroutine(InvKey(false));
                }
            }
        }
        else if (ready && Input.GetKeyDown(kInventory) && !inUI)
        {
            _inventoryManager.OpenUI(InventoryManager.UIType.Inventory);
            inUI = true;
            Mouse.Lock(false);
            StartCoroutine(InvKey(false));
        }
    }

    private IEnumerator InvKey(bool state)
    {
        ready = false;
        yield return new WaitForSecondsRealtime(0.2f);
        while (Input.GetKey(kInventory) == state)
        {
            yield return null;
        }
        ready = true;
    }
    
    private void MoveToSpawn()
    {
        GameObject[] spawns = GameObject.FindGameObjectsWithTag("Respawn");
        int index = Random.Range(0, spawns.Length - 1);
        transform.position = spawns[index].transform.position;
    }

    private void Movement()
    {
        //Checking if the player is grounded
        isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, floorMask, QueryTriggerInteraction.Ignore);
        hitHead = Physics.CheckSphere(headCheacker.position, groundDistance, floorMask, QueryTriggerInteraction.Ignore);
        
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
        if (Input.GetKeyDown(kJump) && isGrounded && canJump)
        {
            isGrounded = false;
            _inputs.y = jumpHeight;
        }
        
        if (Input.GetKey(kForward) && canMove)
        {
            _inputs.z += movementSpeed;    
        }
        else if (Input.GetKey(kBackward) && canMove)
        {
            _inputs.z -= movementSpeed;
        }

        if (Input.GetKey(kLeft) && canMove)
        {
            _inputs.x -= movementSpeed;
        }
        else if (Input.GetKey(kRight) && canMove)
        {
            _inputs.x += movementSpeed;
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
        characterController.Move(transform.TransformVector(_inputs * Time.deltaTime));
        
        //Slowing down the player back to 0
        _inputs.x = Mathf.Lerp(_inputs.x, 0f, breakingSpeed * Time.fixedDeltaTime);
        _inputs.z = Mathf.Lerp(_inputs.z, 0f, breakingSpeed * Time.fixedDeltaTime);
    }

    private void Gravity()
    {
        //Checking for keys being pressed
        if (Input.GetKeyDown(kJump) && isGrounded && canJump)
        {
            
        }
        else if (isGrounded)
        {
            _inputs.y = gravity;
        }
        else if (!isGrounded)
        {
            _inputs.y -= gravityIncrease;
        }
        
        //Because there is no rigidbody, we need to make our own physics
        if (hitHead && _inputs.y > 0)
        {
            _inputs.y = 0;
        }
    }

    private void CameraMovement()
    {
        if (inUI)
            return;
        transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
        rotationY += Input.GetAxis("Mouse Y") * sensitivity;
        rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
        _cameraTransform.localEulerAngles = new Vector3(-rotationY, 0, 0);
    }
    
    public void SetControls()
    {
        List<Binding> bindings = FileManager.GetPlayersConfigAsBindings();

        for (int i = 0; i < bindings.Count; i++)
        {
            Binding lastBind = bindings[i];
            switch (lastBind.commandName)
            {
                case "jump":
                    kJump = lastBind.binding;
                    break;
                case "forward":
                    kForward = lastBind.binding;
                    break;
                case "back":
                    kBackward = lastBind.binding;
                    break;
                case "left":
                    kLeft = lastBind.binding;
                    break;
                case "right":
                    kRight = lastBind.binding;
                    break;
                case "sprint":
                    kSprint = lastBind.binding;
                    break;
                case "inventory":
                    kInventory = lastBind.binding;
                    break;
            }
        }
    }

    private IEnumerator TakeScreenShot()
    {
        miniMap.SetActive(false);
        yield return new WaitForSeconds(1);
        ScreenCapture.CaptureScreenshot(
            string.Format("{0}/screenshot_{1}_{2}_{3}.png", Directory.GetCurrentDirectory(), DateTime.Now.Hour,
                DateTime.Now.Minute, DateTime.Now.Second),
            10);
        
        yield return new WaitForSeconds(1);
        miniMap.SetActive(true);
    }
    
    float pushPower = 2.0f;
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
        {
            return;
        }

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3)
        {
            return;
        }

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.velocity = pushDir * pushPower;
    }
}
