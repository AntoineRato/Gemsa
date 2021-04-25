using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class sc_PlayerController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the camera of the player.")]
    [SerializeField] public Transform playerCamera;
    [Tooltip("Reference to the head of the player.")]
    [SerializeField] private Transform playerHead;

    [Header("Player Movements Configuration")]

    [Header("Inputs")]
    [SerializeField] private string yAxisInput = "Vertical";
    [SerializeField] private string xAxisInput = "Horizontal";
    [SerializeField] private string inputMouseX = "Mouse X";
    [SerializeField] private string inputMouseY = "Mouse Y";
    [SerializeField] private string jumpButton = "Jump";
    [SerializeField] private string walkButton = "Walk";
    [SerializeField] private string crounchButton = "Crounch";

    [Header("Properties")]
    [Tooltip("Self-explanatory.")]
    [SerializeField] private float mouseSensitivity = 1f;
    [Tooltip("Determines how quickly the player accelerates on ground.")]
    [SerializeField] private float groundAcceleration = 100f;
    [Tooltip("Determines how quickly the player accelerates in air. Higher values will make gaining speed by air strafing easier. Surfing also works best with high values.")]
    [SerializeField] private float airAcceleration = 100f;
    [Tooltip("Determines how quickly the camera's rotation matches player input. Lower values cause more delay but higher values can cause jitter.")]
    [SerializeField] private float turnSpeed = 100f;
    [Tooltip("Determines how quickly the camera reaches the player. Lower values cause more delay but higher values can cause jitter")]
    [SerializeField] private float moveSpeed = 100f;
    [Tooltip("Determines the maximum speed on ground when sprinting. The actual speed in game will be generally a little slower due to friction. Note that you can exceed this value by ground strafing if Clamp Ground Speed is turned off.")]
    [SerializeField] private float groundLimitSprint = 12f;
    [Tooltip("Determines the maximum speed on ground when walking.")]
    [SerializeField] private float groundLimitWalk = 8f;
    [Tooltip("Determines the maximum speed on ground when crounching.")]
    [SerializeField] private float groundLimitCrounch = 4f;
    [Tooltip("Determines the maximum speed you can move in air without air strafing. Note that altering this value will change the behaviour of air strafing drastically, with higher values making gaining speed easier.")]
    [SerializeField] private float airLimit = 1f;
    [Tooltip("Self-explanatory.")]
    [SerializeField] private float gravity = 16f;
    [Tooltip("Self-explanatory. Note that higher values will make gaining speed by ground strafing harder.")]
    [SerializeField] private float friction = 6f;
    [Tooltip("Self-explanatory. Note that this is a velocity value, not the actual height.")]
    [SerializeField] private float jumpHeight = 6f;
    [Tooltip("How quickly you must travel upwards to make the controller think you're in air. In other words, the lower this value, the easier it is to slide up slopes.")]
    [SerializeField] private float rampSlideLimit = 5f;
    [Tooltip("Determines on how steep slopes you can walk on.")]
    [SerializeField] private float slopeLimit = 45f;
    [Tooltip("With this setting enabled, you can gain extra height by chaining multiple jumps together, or by jumping while running up a slope.When this setting is disabled, every jump will be the same height.")]
    [SerializeField] private bool additiveJump = true;
    [Tooltip("Determines if you can keep jumping repeatedly by holding down the jump button, or if you have to press it again after each jump.")]
    [SerializeField] private bool autoJump = true;
    [Tooltip("Limits your maximum ground speed on ground to the Ground Limit value, making gaining speed by ground strafing, and techniques such as circle jumping impossible.")]
    [SerializeField] private bool clampGroundSpeed = false;
    [Tooltip("Clamps your speed to the Ground Limit value each time you land, making you unable to maintain the speed gained by air strafing.")]
    [SerializeField] private bool disableBunnyHopping = false;

    private Rigidbody rb;
    private sc_PlayerProperties playerProperties;

    private Vector3 vel;
    private Vector3 inputDir;
    private Vector3 _inputRot;
    private Vector3 groundNormal;
    //private Vector3 oldPosition;

    private Quaternion oldRotationHead;
    private Quaternion oldRotationCamera;

    private Animator playerAnimator;

    private bool onGround = false;
    private bool jumpPending = false;
    private bool ableToJump = true;
    private bool isWalk = false;
    private bool isCrounch = false;
    private float groundLimit;

    public Vector3 InputRot { get => _inputRot; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerProperties = this.GetComponent<sc_PlayerProperties>();
        playerAnimator = this.GetComponent<Animator>();
        groundLimit = groundLimitSprint;
    }

    void Start()
    {
        // Lock cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //oldPosition = playerHead.position;
        oldRotationHead = playerHead.rotation;
        oldRotationCamera = playerCamera.rotation;
    }


    private void Update()
    {
        if (!playerProperties.isDead)
        {
            MouseLook();
            GetMovementInput();
        }

        ToggleCursor();
    }

    private void LateUpdate()
    {
        if (!playerProperties.isDead)
        {
            UpdateAnimator();

            //playerHead.position = Vector3.Lerp(oldPosition, playerHead.position, moveSpeed * Time.deltaTime);
            playerHead.rotation = Quaternion.Lerp(oldRotationHead, Quaternion.Euler(new Vector3(Mathf.Clamp(this.InputRot.x, -60f, 50f), this.InputRot.y, this.InputRot.z)), turnSpeed * Time.deltaTime);
            playerCamera.rotation = Quaternion.Lerp(oldRotationCamera, Quaternion.Euler(this.InputRot), turnSpeed * Time.deltaTime);

            //oldPosition = playerHead.position;
            oldRotationHead = playerHead.rotation;
            oldRotationCamera = playerCamera.rotation;
        }
    }

    private void ToggleCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!Cursor.visible && Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    private void FixedUpdate()
    {
        vel = rb.velocity;

        // Clamp speed if bunnyhopping is disabled
        if (disableBunnyHopping && onGround)
        {
            if (vel.magnitude > groundLimit)
                vel = vel.normalized * groundLimit;
        }

        // Jump
        if (jumpPending && onGround)
        {
            Jump();
        }

        // We use air physics if moving upwards at high speed
        if (rampSlideLimit >= 0f && vel.y > rampSlideLimit)
        {
            onGround = false;
            playerAnimator.SetBool("onGround", false);
        }

        if (onGround)
        {
            // Rotate movement vector to match ground tangent
            inputDir = Vector3.Cross(Vector3.Cross(groundNormal, inputDir), groundNormal);

            GroundAccelerate();
            ApplyFriction();
        }
        else
        {
            ApplyGravity();
            AirAccelerate();
        }

        rb.velocity = vel;

        // Reset onGround before next collision checks
        onGround = false;
        // playerAnimator.SetBool("onGround", false);
        groundNormal = Vector3.zero;
    }

    void GetMovementInput()
    {
        float x = Input.GetAxisRaw(xAxisInput);
        float z = Input.GetAxisRaw(yAxisInput);

        inputDir = transform.rotation * new Vector3(x, 0f, z).normalized;

        if (Input.GetButtonDown(jumpButton))
            jumpPending = true;

        if (Input.GetButtonUp(jumpButton))
            jumpPending = false;

        if (Input.GetButtonDown(crounchButton))
        {
            isCrounch = true;
            playerAnimator.SetBool("crounch", true);
            groundLimit = groundLimitCrounch;
        }
        else if (Input.GetButtonUp(crounchButton))
        {
            isCrounch = false;
            playerAnimator.SetBool("crounch", false);
            if (isWalk)
                groundLimit = groundLimitWalk;
            else
                groundLimit = groundLimitSprint;
        }
        
        if (Input.GetButtonDown(walkButton))
        {
            isWalk = true;
            playerAnimator.SetBool("walk", true);
            if(!isCrounch)
                groundLimit = groundLimitWalk;
        }
        else if (Input.GetButtonUp(walkButton))
        {
            isWalk = false;
            playerAnimator.SetBool("walk", false);
            if(!isCrounch)
                groundLimit = groundLimitSprint;
        }
    }

    private void UpdateAnimator()
    {
        //Vector3 localPlayerVelocity = this.transform.InverseTransformDirection(this.vel);
        playerAnimator.SetFloat("xAxis", Input.GetAxisRaw(xAxisInput));
        playerAnimator.SetFloat("zAxis", Input.GetAxisRaw(yAxisInput));
    }

    void MouseLook()
    {
        _inputRot.y += Input.GetAxisRaw(inputMouseX) * mouseSensitivity;
        _inputRot.x -= Input.GetAxisRaw(inputMouseY) * mouseSensitivity;

        if (_inputRot.x > 90f)
            _inputRot.x = 90f;
        if (_inputRot.x < -90f)
            _inputRot.x = -90f;

        transform.rotation = Quaternion.Euler(0f, _inputRot.y, 0f);
    }

    private void GroundAccelerate()
    {
        float addSpeed = groundLimit - Vector3.Dot(vel, inputDir);

        if (addSpeed <= 0)
            return;

        float accelSpeed = groundAcceleration * Time.deltaTime;

        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;

        vel += accelSpeed * inputDir;

        if (clampGroundSpeed)
        {
            if (vel.magnitude > groundLimit)
                vel = vel.normalized * groundLimit;
        }
    }

    private void AirAccelerate()
    {
        Vector3 hVel = vel;
        hVel.y = 0;

        float addSpeed = airLimit - Vector3.Dot(hVel, inputDir);

        if (addSpeed <= 0)
            return;

        float accelSpeed = airAcceleration * Time.deltaTime;

        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;

        vel += accelSpeed * inputDir;
    }

    private void ApplyFriction()
    {
        vel *= Mathf.Clamp01(1 - Time.deltaTime * friction);
    }

    private void Jump()
    {
        if (!ableToJump)
            return;

        if (vel.y < 0f || !additiveJump)
            vel.y = 0f;

        vel.y += jumpHeight;
        onGround = false;

        if (!autoJump)
            jumpPending = false;
        
        playerAnimator.SetBool("jump", true);
        playerAnimator.SetBool("onGround", false);

        StartCoroutine(JumpTimer());
    }



    private void ApplyGravity()
    {
        vel.y -= gravity * Time.deltaTime;
    }

    private void OnCollisionStay(Collision other)
    {
        // Check if any of the contacts has acceptable floor angle
        foreach (ContactPoint contact in other.contacts)
        {
            if (contact.normal.y > Mathf.Sin(slopeLimit * (Mathf.PI / 180f) + Mathf.PI / 2f))
            {
                groundNormal = contact.normal;

                if (ableToJump && !onGround)
                {
                    onGround = true;
                    playerAnimator.SetBool("onGround", true);
                }

                return;
            }
        }
    }

    // This is for avoiding multiple consecutive jump commands before leaving ground
    private IEnumerator JumpTimer()
    {
        ableToJump = false;
        yield return new WaitForSeconds(0.1f);
        playerAnimator.SetBool("jump", false);
        ableToJump = true;
    }

    private void OnDestroy()
    {
        Destroy(this.rb);
    }
}
