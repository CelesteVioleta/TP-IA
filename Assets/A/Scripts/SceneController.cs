using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private string levelScene = "Scene_Level";
    [SerializeField] private string menuScene = "Scene_Menu";

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == levelScene &&
            Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Scene_Menu");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Scene_Level");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Scene_Menu");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}