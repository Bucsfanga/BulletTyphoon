using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class GameManager : MonoBehaviour
{
    // UI Panels
    public GameObject mainMenu;
    public GameObject hud;
    public GameObject pauseMenu;
    public GameObject settingsMenu;


    // HUD Elements
    public RectTransform healthFill; 
    public TextMeshProUGUI ammoCounter;
    public TextMeshProUGUI interactPrompt;

    private float fullWidth;

    // Settings Menu Elements
    public Slider volumeSlider;

    private bool isPaused = false;
    private AudioSource audioSource;



    void Start()
    {
        fullWidth = healthFill.sizeDelta.x;

        audioSource = Camera.main.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            volumeSlider.value = audioSource.volume;
            volumeSlider.onValueChanged.AddListener(UpdateVolume);
        }
    }



    // ------------------------------
    // Main Menu Functions
    // ------------------------------

    public void StartGame()
    {
        mainMenu.SetActive(false);  // Hide Main Menu
        hud.SetActive(true);  // Show HUD
        SceneManager.LoadScene("MainGame");
    }

    public void OpenSettingsFromMainMenu()
    {
        mainMenu.SetActive(false);  // Hide Main Menu
        settingsMenu.SetActive(true);  // Show Settings Menu
    }

    public void QuitGame()
    {
        Application.Quit();  // Quit the application
        Debug.Log("Game Quit.");
    }

    // ------------------------------
    // HUD Functions
    // ------------------------------


    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        ammoCounter.text = $"Ammo: {currentAmmo}/{maxAmmo}";
    }

    public void ShowInteractionPrompt(string message)
    {
        interactPrompt.text = message;
        interactPrompt.gameObject.SetActive(true);
    }

    public void HideInteractionPrompt()
    {
        interactPrompt.gameObject.SetActive(false);
    }

    // ------------------------------
    // Pause Menu Functions
    // ------------------------------

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;  // Pause the game
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;  // Resume the game
        isPaused = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // Reload current scene
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Leah");
    }
    // ------------------------------
    // Settings Menu Functions
    // ------------------------------
    public void UpdateVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;  // Update audio source volume
        }
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);  // Show Pause Menu after closing settings
    }
	
	    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        float healthPercentage = currentHealth / maxHealth;  // Calculate percentage (0 to 1)
        healthFill.sizeDelta = new Vector2(fullWidth * healthPercentage, healthFill.sizeDelta.y);  // Adjust width of health fill
    }
}
