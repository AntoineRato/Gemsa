using UnityEngine;

public class sc_SmoothPlayerCamera : MonoBehaviour
{
    /*
    //[SerializeField] private float height = 0.5f;
    //[SerializeField] private float moveSpeed = 100f;
    [SerializeField] private float turnSpeed = 100f;
    //[SerializeField] private float distanceLimit = 1f;
    [SerializeField] private sc_PlayerController controller;

    private Vector3 oldPos;
    private Quaternion oldRot;

    
    private void Start() {
        //oldPos = transform.position;
        //oldRot = transform.rotation;
    }

    private void LateUpdate() {
        //Vector3 targetPos = controller.transform.position + new Vector3(0f, height, 0f);

        // Lerp position
        //transform.position = Vector3.Lerp(oldPos, targetPos, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(oldRot, Quaternion.Euler(controller.InputRot), turnSpeed * Time.deltaTime);

        //if (Vector3.Distance(transform.position, targetPos) > distanceLimit) {
        //    transform.position = targetPos;
        //}

        //oldPos = transform.position;
        oldRot = transform.rotation;
    }
    */
}
