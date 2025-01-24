using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // UI Panels
    public GameObject mainMenu;
    public GameObject hud;
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject player;
    public playerController playerScript;
    public GameObject damagePanel;
    public Image playerHPBar;

    public TMP_Text goalCountText;

    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    // HUD Elements
    public RectTransform healthFill;

    public TextMeshProUGUI ammoCounter;
    public Image[] ammoBullets; // Array to hold the bullet images
    public float bulletAlpha = 0.3f; // Alpha value for empty bullets

    public TextMeshProUGUI interactPrompt;

    private float fullWidth;
    private Light directionalLight; //variable for finding light and its intensity.
    private float originalLightIntensity;

    // Settings Menu Elements
    public Slider volumeSlider;

    public bool isPaused = false;
    private AudioSource audioSource;

    int goalCount;
    public int goalCheckpoint = 0;
    public TMP_Text incomingWarningText;
    public GameObject submergedOverlay;

    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();

        //finding the direction light in the scene
        directionalLight = Object.FindAnyObjectByType<Light>();
        if (directionalLight != null)
        {
            originalLightIntensity = directionalLight.intensity;
        }
        else
        {
            Debug.LogWarning("No directional light found in the scene.");
        }
    }

    void Start()
    {
        fullWidth = healthFill.sizeDelta.x;

      //  audioSource = Camera.main.GetComponent<AudioSource>();
      //  if (audioSource != null)
        {
        //    volumeSlider.value = audioSource.volume;
       //     volumeSlider.onValueChanged.AddListener(UpdateVolume);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }
    }

    // ------------------------------
    // Main Menu Functions
    // ------------------------------

    public void StartGame()
    {
        mainMenu.SetActive(false);  // Hide Main Menu
        hud.SetActive(true);  // Show HUD
        SceneManager.LoadScene("UnitTest");  // Load game scene
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

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
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
                bulletColor.a = 1f; // change opacity
            }
            else
            {
                bulletColor.a = bulletAlpha; // Reduced opacity for empty bullets
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
       // if (audioSource != null)
       // {
       //     audioSource.volume = volume;  // Update audio source volume
        //}
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);  // Show Pause Menu after closing settings
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
