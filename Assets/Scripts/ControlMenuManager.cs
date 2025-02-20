using UnityEngine;

public class ControlMenuManager : MonoBehaviour
{
    public GameObject controlMenu;


   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && controlMenu.activeSelf)
        {
            controlMenu.SetActive(false);
            Time.timeScale = 1;  // Resume game
        }
    }

}
