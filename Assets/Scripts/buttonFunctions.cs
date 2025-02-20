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

    public void nextLevel()
    {
        GameManager.instance.NextLevel();
    }

    // Back button for settings menu
    public void BackToPauseMenuFromSettings()
    {
        GameManager.instance.CloseSettings();
        
    }

    // Back button for credits menu
    public void BackToMainMenuFromCredits()
    {
         GameManager.instance.CloseCredits();
        
    }
    public void BackToMainMenuFromPause()
    {
        GameManager.instance.ReturnToMainMenu();
    }
}
