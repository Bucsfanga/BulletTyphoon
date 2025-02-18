using System.Collections;
using TMPro;
using Unity.VisualScripting;
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
    public GameObject buttonInteract;
    public Image playerHPBar;


    public TMP_Text buttonInfo;
    public TMP_Text goalCountText;

    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuClassifiedDoc;
    [SerializeField] GameObject noticeBanner;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuMain;
    [SerializeField] GameObject menuSettings;
    [SerializeField] GameObject menuCredits;
    [SerializeField] GameObject menuControls;
    [SerializeField] GameObject aimReticle;
    private GameObject lastMenu;

    // HUD Elements
    public RectTransform healthFill;
    public Image[] ammoBullets; // Array to hold the bullet images
    public float bulletAlphaLoaded = 0.30f; // Alpha value for loaded bullets
    public float bulletAlphaSpented = -5.0f; // Alpha value for empty bullets

    public TextMeshProUGUI loseMessageText;
    public TextMeshProUGUI interactPrompt;
    [SerializeField] TextMeshProUGUI ammoCounterText;

    private float fullWidth;
    private Light directionalLight; //variable for finding light and its intensity.
    private float originalLightIntensity;
    private RawImage[] _documents; // Doanld added for Classifiecation menu only 

    // Settings Menu Elements
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider backgroundVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    public Slider volumeSlider;

    public bool isPaused;
    

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
        StartCoroutine(InitializeVolumeSliders());
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
            audioManager.instance.StopMenuMusic();

            if (player != null)
            {
                player.SetActive(true);
            }
        }
        else
        {
            initializeMainMenu(); // Initialize main menu for fresh start
        }

        _documents = menuClassifiedDoc.GetComponentsInChildren<RawImage>();

        GameState.isRestarting = false; // Reset flag
        GameState.isNextLevel = false; // Reset flag

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
        populateBanner();
        PopulateClassifiedWIn();
    }

    public void updateAmmoCounter(int currentAmmo, int maxAmmo, int totalAmmo)
    {
        if (ammoCounterText != null)
        {
            ammoCounterText.text = $"{currentAmmo} / {maxAmmo} | {totalAmmo}";
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
        audioManager.instance.PlayUIClick();
        stateUnpause();
        menuMain.SetActive(false);  // Hide Main Menu
        StartCoroutine(ContrtolsScreen());
        hud.SetActive(true);  // Show HUD
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;  // Resume the game
        isPaused = false;
        audioManager.instance.StopMenuMusic(); //Ian add - fade out the menu music as the game starts

        if (player != null)
        {
            player.SetActive(true);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void QuitGame()
    {
        audioManager.instance.PlayUIClick();
        Application.Quit();  // Quit the application
        Debug.Log("Game Quit.");
    }

    public void youLose(string reason)
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
        Debug.Log("You lost! Reason: " + reason); // Show reason 
        audioManager.instance.PlayLoseMenuMusicAudio();
        
        if (loseMessageText != null)
        {
            loseMessageText.text = reason; // Display why the player lost
        }
    }
    public void statePause()
    {
        isPaused = true;
        hud.SetActive(false);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = false;
        hud.SetActive(true);
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
        audioManager.instance.PlayUIClick();
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
        audioManager.instance.PlayUIClick();
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

    //Animation not working Donald troubleshooting in next Sprint Beta
    public void ShootAnim()
    {
        Debug.Log("Triggering animation...");
        aimReticle.GetComponent<Animator>().Play("AimRecticleShoot");
    }

    // ------------------------------
    // Pause Menu Functions
    // ------------------------------


    public void PauseGame()
    {
        hud.SetActive(false);
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;  // Pause the game
        isPaused = true;
    }

    public void ResumeGame()
    {
        audioManager.instance.PlayUIClick();
        pauseMenu.SetActive(false);
        hud.SetActive(true);
        Time.timeScale = 1f;  // Resume the game
        isPaused = false;
    }

    public void RestartGame()
    {
        audioManager.instance.PlayUIClick();
        Debug.Log("Restarting Scene: " + SceneManager.GetActiveScene().name); // Verify correct scene is reloading
        GameState.isRestarting = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // Reload current scene
    }

    public void NextLevel()
    {
        audioManager.instance.PlayUIClick();
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

    public void ShowSettings()
    {
        audioManager.instance.PlayUIClick();
        // Store the currently active menu before switching to settings
        lastMenu = menuActive;

        // If the Main Menu is active, hide it
        if (menuMain.activeSelf)
        {
            menuMain.SetActive(false);
        }
        // If the Pause Menu is active, hide it and the HUD
        else if (menuActive == menuPause)
        {
            menuPause.SetActive(false);
            hud.SetActive(false);
        }

        // Open the Settings Menu
        menuSettings.SetActive(true);
        menuActive = menuSettings;

    }

    public void CloseSettings()
    {
        audioManager.instance.PlayUIClick();
        if (menuActive == menuSettings)
        {
            menuSettings.SetActive(false);

            // Return to the previous menu (Main Menu or Pause Menu)
            if (lastMenu != null)
            {
                lastMenu.SetActive(true);
                menuActive = lastMenu;
            }

            // If returning to Pause Menu, re-enable HUD
            if (lastMenu == menuPause)
            {
                hud.SetActive(true);
            }
        }
    }

    private IEnumerator InitializeVolumeSliders()
    {
        yield return null; //wait a frame to give audio mixer time to initialize

        if (masterVolumeSlider != null)
        {
            Debug.Log("Creating Master Volume Slider!");
            masterVolumeSlider.value = audioManager.instance.GetVolumeFromMixer("MasterVolume");
            masterVolumeSlider.onValueChanged.AddListener(HandleMasterVolumeChange);
        }
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = audioManager.instance.GetVolumeFromMixer("MusicVolume");
            musicVolumeSlider.onValueChanged.AddListener(HandleMusicVolumeChange);
        }
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = audioManager.instance.GetVolumeFromMixer("SFXVolume");
            sfxVolumeSlider.onValueChanged.AddListener(HandleSFXVolumeChange);
        }
        if (backgroundVolumeSlider != null)
        {
            backgroundVolumeSlider.value = audioManager.instance.GetVolumeFromMixer("BackgroundVolume");
            backgroundVolumeSlider.onValueChanged.AddListener(HandleBackgroundVolumeChange);
        }
    }

    private void HandleMasterVolumeChange(float val)
    {
        audioManager.instance.SetMasterVolume(val);
    }
    private void HandleMusicVolumeChange(float val)
    {
        audioManager.instance.SetMusicVolume(val);
    }
    private void HandleSFXVolumeChange(float val)
    {
        audioManager.instance.SetSFXVolume(val);
    }
    private void HandleBackgroundVolumeChange(float val)
    {
        audioManager.instance.SetBackgroundVolume(val);
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

    // ------------------------------
    // Disply Controls on screen
    // ------------------------------
    private IEnumerator ContrtolsScreen()
    {
        menuControls.SetActive(true);
        yield return new WaitForSeconds(2);
        menuControls.SetActive(false);
    }

    // ------------------------------
    // Testing aera for Notice Banner by Donald
    // ------------------------------
    private void populateBanner()
    {
        if (Input.GetKeyDown("k"))
        {
            noticeBanner.GetComponent<NoticeBanner>().Notice(0);
        }

        if (Input.GetKeyDown("l"))
        {
            noticeBanner.GetComponent<NoticeBanner>().Notice(1);
        }
    }

    private void PopulateClassifiedWIn()
    {

        if (Input.GetKeyDown("n"))
        {
            statePause();
            _documents[0].gameObject.SetActive(true);
            _documents[1].gameObject.SetActive(false);
            menuActive = menuClassifiedDoc;
            menuActive.SetActive(true);
            noticeBanner.GetComponent<NoticeBanner>().Notice(2);
        }

        if (Input.GetKeyDown("m"))
        {
            statePause();
            _documents[1].gameObject.SetActive(true);
            _documents[0].gameObject.SetActive(false);
            menuActive = menuClassifiedDoc;
            menuActive.SetActive(true);
            noticeBanner.GetComponent<NoticeBanner>().Notice(2);
        }

        if(Input.GetKeyDown("enter") && (menuActive == menuClassifiedDoc))
        {
            menuActive.SetActive(false);
            noticeBanner.GetComponent<NoticeBanner>()._noticeBanner.enabled = false;
            initializeMainMenu();
        }
    }
}
