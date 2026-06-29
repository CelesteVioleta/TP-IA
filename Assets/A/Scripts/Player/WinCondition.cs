using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Scene_Win");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
