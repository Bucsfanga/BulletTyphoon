using UnityEngine;

public class cameraControerler : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    float rotX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
     
        // Get inputs
        float mouseX =Input.GetAxis("mouseX X") * sens * Time.deltaTime;
        float mouseY =Input.GetAxis("mouseX Y") * sens * Time.deltaTime;

        // Tie the mouseY to the rotX of the camera - look up and down
        if (invertY)
            rotX += mouseY;
        else
            rotX -= mouseY;

        // Clamp the cmera on the x-Axis
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        // Rotate the camera on the X-Axis
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        // Rotate the player on the Y-Axis - look left and right
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
