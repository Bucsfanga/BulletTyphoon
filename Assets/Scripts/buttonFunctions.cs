using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{

    public void resume()
    {
        GameManager.instance.stateUnpause();
    }

    public void restart()
    {
        GameManager.instance.RestartGame();
    }

    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void ShowCredits()
    {
        GameManager.instance.ShowCredits();
    }

    public void ShowSettings()
    {
        GameManager.instance.ShowSettings();
    }

    public void startGame()
    {
        GameManager.instance.StartGame();
    }
}
