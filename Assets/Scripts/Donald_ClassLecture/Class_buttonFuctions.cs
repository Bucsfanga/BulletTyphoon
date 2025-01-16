using UnityEngine;
using UnityEngine.SceneManagement;

public class Class_buttonFuctions : MonoBehaviour
{
    public void resume()
    {
        Class_gameManager.instance.stateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Class_gameManager.instance.stateUnpause();
    }

    public void quit()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}