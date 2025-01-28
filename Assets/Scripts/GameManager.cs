using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // UI Panels
    public GameObject hud;
    public GameObject pauseMenu;
    public GameObject player;
    public playerController playerScript;
    public GameObject damagePanel;
    public Image playerHPBar;

    public TMP_Text goalCountText;

    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuMain;
    [SerializeField] GameObject menuSettings;
    [SerializeField] GameObject menuCredits;

    // HUD Elements
    public RectTransform healthFill;

    public TextMeshProUGUI ammoCounter;
    public Image[] ammoBullets; // Array to hold the bullet images
    public float bulletAlphaLoaded = 0.60f; // Alpha value for loaded bullets
    public float bulletAlphaSpented = -5.0f; // Alpha value for empty bullets

    public TextMeshProUGUI interactPrompt;

    private float fullWidth;
    private Light directionalLight; //variable for finding light and its intensity.
    private float originalLightIntensity;

    // Settings Menu Elements
    public Slider volumeSlider;

    public bool isPaused;
    private AudioSource audioSource;

    int goalCount;
    public int goalCheckpoint = 0;
    public TMP_Text incomingWarningText;
    public GameObject submergedOverlay;

    public static class GameState
    {
        public static bool isRestarting;
        public static bool isNextLevel;
        public static bool showCredits;
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (GameState.showCredits)
        {
            initializeMainMenu();
            GameState.showCredits = false; // Reset flag
            ShowCredits(); // Trigger credit display
            return;
        }

        if (GameState.isRestarting || GameState.isNextLevel)
        {
            // Skip initializing main menu and unpause game
            hud.SetActive(true);
            pauseMenu.SetActive(false);
            menuMain.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            if (player != null)
            {
                player.SetActive(true);
            }
        }
        else
        {
            initializeMainMenu(); // Initialize main menu for fresh start
        }

        GameState.isRestarting = false; // Reset flag
        GameState.isNextLevel = false; // Reset flag

        // Check for null references before proceeding
        if (audioManager.instance != null && volumeSlider != null)
        {
            // Initialize the slider with the current background audio volume
            volumeSlider.value = audioManager.instance.GetBackgroundAudioVolume();

            // Add a listener to update the volume when the slider value changes
            volumeSlider.onValueChanged.AddListener(UpdateVolume);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == menuPause)
            {
                stateUnpause();
            }
            else if (menuActive == menuCredits)
            {
                CloseCredits();
            }
            else if (menuActive == menuSettings)
            {
                CloseSettings();
            }
            else if (menuActive == null && hud.activeSelf)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
        }
    }

    private void initializeMainMenu()
    {
        Debug.Log("Initializing Main Menu!");
        menuMain.SetActive(true);
        menuActive = menuMain;
        hud.SetActive(false);
        pauseMenu.SetActive(false);
        statePause();

        if (player != null)
        {
            player.SetActive(false);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // ------------------------------
    // Main Menu Functions
    // ------------------------------

    public void StartGame()
    {
        stateUnpause();
        menuMain.SetActive(false);  // Hide Main Menu
        hud.SetActive(true);  // Show HUD
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;  // Resume the game
        isPaused = false;

        if (player != null)
        {
            player.SetActive(true);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }



    public void OpenSettingsFromMainMenu()
    {
        menuMain.SetActive(false);  // Hide Main Menu
        menuSettings.SetActive(true);  // Show Settings Menu
    }

    public void QuitGame()
    {
        Application.Quit();  // Quit the application
        Debug.Log("Game Quit.");
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
    }

    // Track number of living enemies in level
    public void updateGameGoal(int amount)
    {
        goalCount += amount;
        goalCountText.text = goalCount.ToString("F0");

        //if (goalCount <= 0)
        //{
        //    statePause();
        //    menuActive = menuWin;
        //    menuActive.SetActive(true);
        //}
    }

    // Track win condition of player reaching goal position on level
    public void updateGameWinCondition(int amount)
    {
        goalCheckpoint += amount;

        if (goalCheckpoint >= 1)
        {
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }
    public void ShowCredits()
    {
        if (menuActive == menuMain)
        {
            menuMain.SetActive(false);
            menuCredits.SetActive(true);
            menuActive = menuCredits;

            // Trigger credit scroller
            creditsScroller scroller = menuCredits.GetComponentInChildren<creditsScroller>();
            if (scroller != null)
            {
                scroller.startScrolling();
            }
        }
    }

    public void CloseCredits()
    {
        if (menuActive == menuCredits)
        {
            // Reset credit scroller
            creditsScroller scroller = menuCredits.GetComponentInChildren<creditsScroller>();
            if (scroller != null)
            {
                scroller.resetCredits();
            }

            menuCredits.SetActive(false);
            menuMain.SetActive(true);
            menuActive = menuMain;
        }
    }

    // ------------------------------
    // HUD Functions
    // ------------------------------


    // setting up the bullet count functions
    public void UpdateAmmoBorder(int ammoCur, int ammoMax)
    {
        // Calculate how many bullets should be shown as "filled"
        int bulletsToShow = Mathf.CeilToInt((float)ammoCur / ammoMax * ammoBullets.Length);

        // Update each bullet image
        for (int i = 0; i < ammoBullets.Length; i++)
        {
            Color bulletColor = ammoBullets[i].color;

            // If this bullet should be "filled"
            if (i < bulletsToShow)
            {
                bulletColor.a = bulletAlphaLoaded; // change opacity
            }
            else
            {
                bulletColor.a = bulletAlphaSpented; // Reduced opacity for empty bullets
            }

            ammoBullets[i].color = bulletColor;
        }
    }


    public void UpdateAmmo(int ammoCur, int ammoMax)
    {
        ammoCounter.text = $"Ammo: {ammoCur}/{ammoMax}";
        UpdateAmmoBorder(ammoCur, ammoMax);
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

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        float healthPercentage = currentHealth / maxHealth;  // Calculate percentage (0 to 1)
        healthFill.sizeDelta = new Vector2(fullWidth * healthPercentage, healthFill.sizeDelta.y);  // Adjust width of health fill
    }

    // ------------------------------
    // Pause Menu Functions
    // ------------------------------


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
        Debug.Log("Restarting Scene: " + SceneManager.GetActiveScene().name); // Verify correct scene is reloading
        GameState.isRestarting = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // Reload current scene
    }

    public void NextLevel()
    {
        Debug.Log("Loading next level");
        GameState.isNextLevel = true;
        Time.timeScale = 1f;

        // Load next scene based on build index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if next scene index is within bounds
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No more levels to load. Play credits.");
            GameState.isNextLevel = false;
            GameState.showCredits = true;
            SceneManager.LoadScene(0);
        }
    }

    // ------------------------------
    // Settings Menu Functions
    // ------------------------------
    public void UpdateVolume(float volume)
    {

        audioManager.instance.SetBackgroundAudioVolume(volume);
    }

    public void ShowSettings()
    {
        if (menuActive == menuPause)
        {
            menuPause.SetActive(false);
            hud.SetActive(false);
            menuSettings.SetActive(true);
            menuActive = menuSettings;
        }
    }

    public void CloseSettings()
    {
        if (menuActive == menuSettings)
        {
            menuSettings.SetActive(false);
            hud.SetActive(true);
            menuPause.SetActive(true);  // Show main menu after closing settings
            menuActive = menuPause;
        }
    }

    // ------------------------------
    // Weather Changer Function
    // ------------------------------
    public void setStormLighting(bool isStorm)
    {
        if (directionalLight != null)
        {
            if (isStorm)
            {
                directionalLight.intensity = originalLightIntensity * 0.5f; // This dims the light for the storm.

            }
            else
            {
                directionalLight.intensity = originalLightIntensity;// This returns the light to its original spot when storm is not active.
            }
        }
    }
}
