using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartSceneTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
