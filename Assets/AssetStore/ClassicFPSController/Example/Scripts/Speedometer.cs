using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    private Text text;

    private void Start() {
        text = GetComponent<Text>();   
    }

    private void LateUpdate() {
        Vector3 hVel = rb.velocity;
        hVel.y = 0;

        text.text = hVel.magnitude.ToString("0.0");
    }
}
