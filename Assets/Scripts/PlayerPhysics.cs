using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    [SerializeField] private GameObject playerHead;

    private Vector3 playerVelocity;
    private Vector3 playerHorizontalRotation;
    private Vector3 playerVerticalRotation;
    private Vector3 playerJumpForce;
    private Rigidbody playerRigidbody;

    //MonoBehaviour functions
    private void Awake()
    {
        this.playerRigidbody = this.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyRotation();
        ApplyJump();
    }

    //Interactives functions
    public void MovePlayer(Vector3 newVelocityValue)
    {
        this.playerVelocity = newVelocityValue;
    }

    public void RotatePlayer(Vector3 newHorizontalRotationValue, Vector3 newVerticalRotationValue)
    {
        this.playerHorizontalRotation = newHorizontalRotationValue;
        this.playerVerticalRotation = newVerticalRotationValue;
    }

    public void Jump(float jumpForceValue)
    {
        playerJumpForce = Vector3.up * jumpForceValue;
    }

    //Apply physics functions
    private void ApplyMovement()
    {
        if (this.playerVelocity != Vector3.zero)
        {
            this.playerRigidbody.MovePosition(this.playerRigidbody.position + this.playerVelocity * Time.fixedDeltaTime);
        }
    }

    private void ApplyJump()
    {
        if (this.playerJumpForce != Vector3.zero)
        {
            this.playerRigidbody.AddForce(this.playerJumpForce, ForceMode.Impulse);
            this.playerJumpForce = Vector3.zero;
        }

    }

    private void ApplyRotation()
    {
        this.playerRigidbody.MoveRotation(this.playerRigidbody.rotation * Quaternion.Euler(this.playerHorizontalRotation));
        playerHead.transform.Rotate(-playerVerticalRotation);
    }
}
