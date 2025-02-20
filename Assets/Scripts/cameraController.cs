using UnityEngine;

public class cameraController : MonoBehaviour
{
    public static cameraController instance;

    [SerializeField][Range(0,2)] public float sens = 1f;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;
    float rotX;

    private GameManager gameManager;

    void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        // Set up the singleton instance
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Only set default sensitivity if no saved value exists
        if (!PlayerPrefs.HasKey("MouseSensitivity"))
        {
            PlayerPrefs.SetFloat("MouseSensitivity", sens);
            PlayerPrefs.Save();
        }
        else
        {
            // Load saved sensitivity immediately
            sens = Mathf.Clamp(PlayerPrefs.GetFloat("MouseSensitivity"), 0f, 2f);
        }
    }

    private void Start()
    {
        gameManager = GameManager.instance;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager != null && gameManager.isPaused)
        {
            return;
        }
        // Get inputs
        float mouseX = Input.GetAxis("Mouse X") * sens; //* Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sens; //* Time.deltaTime;

        // Tie the mouseY to the rotX of the camera - look up and down
        if (invertY)
            rotX += mouseY;
        else
            rotX -= mouseY;

        // Clamp the camera on the x-Axis
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        // Rotate the camera on the X-Axis
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        // Rotate the player on the Y-Axis - look left and right
        transform.parent.Rotate(Vector3.up * mouseX);
    }

    public void SetLookSensitivity(float val)
    {
        sens = val;
        PlayerPrefs.SetFloat("MouseSensitivity", sens);
        PlayerPrefs.Save();
    }

    public float GetLookSensitivity()
    {
        return sens;
    }
}
