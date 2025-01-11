using UnityEngine;

public class cameraController : MonoBehaviour
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
        //get input
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;

        //tie the mouseY to the rotX of the camera
        if(invertY)
            rotX += mouseY;
        else
            rotX -= mouseY;
        // clamp camera on the X-axis
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);
        // rotate camera on x-axis
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);
        // rotate player on the Y-axis - look left and right
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
