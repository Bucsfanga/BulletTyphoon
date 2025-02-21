using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    public GameObject firstSelectedButton; 

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    private void Update()
    {
        // Ensure keyboard navigation works if no UI element is selected
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }
}
