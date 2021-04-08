using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(PlayerPhysics))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 4f;
    [SerializeField] private float mouseSensitivityX = 3f;
    [SerializeField] private float mouseSensitivityY = 3f;
    [SerializeField] private float jumpForce = 100f;

    private PlayerPhysics _playerPhysics;

    private void Awake()
    {
        this._playerPhysics = this.GetComponent<PlayerPhysics>();
    }

    // Update is called once per frame
    void Update()
    {
        //Keyboard
        float xAxisMovementInput = Input.GetAxis("Horizontal");
        float yAxisMovementInput = Input.GetAxis("Vertical");

        Vector3 velocity = ((this.transform.right * xAxisMovementInput) + (this.transform.forward * yAxisMovementInput)) * playerSpeed;

        this._playerPhysics.MovePlayer(velocity);

        //Mouse
        float yAxisMouseInput = Input.GetAxisRaw("Mouse X");
        float xAxisMouseInput = Input.GetAxisRaw("Mouse Y");

        Vector3 horizontalRotation = new Vector3(0, yAxisMouseInput, 0) * mouseSensitivityX;
        Vector3 verticalRotation = new Vector3(xAxisMouseInput, 0, 0) * mouseSensitivityY;

        this._playerPhysics.RotatePlayer(horizontalRotation, verticalRotation);

        if(Input.GetButtonDown("Jump"))
        {
            this._playerPhysics.Jump(jumpForce);
        }
    }
}
