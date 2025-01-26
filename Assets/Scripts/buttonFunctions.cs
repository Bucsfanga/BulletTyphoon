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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.stateUnpause();
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
    public void BackToMainMenu()
    {
        GameManager.instance.BackToMainMenu();
    }
    // Function for the Quit to Main Menu button
    public void QuitToMainMenu()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.QuitToMainMenu();
        }

    }
}
