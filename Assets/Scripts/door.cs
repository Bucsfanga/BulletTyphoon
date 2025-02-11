using UnityEngine;

public class door : MonoBehaviour
{
    [SerializeField] GameObject model;
    [SerializeField] string buttonInfo;

    bool inTrigger;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inTrigger)
        {
            if (Input.GetButtonDown("Interact"))
            {
                model.SetActive(false);
                GameManager.instance.buttonInteract.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        iInteract open = other.GetComponent<iInteract>();

        if(open != null)
        {
            inTrigger = true;
            GameManager.instance.buttonInteract.SetActive(true);
            GameManager.instance.buttonInfo.text = buttonInfo;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        inTrigger= false;

        iInteract open = other.GetComponent<iInteract>();

        if (open != null)
        {
            model.SetActive(true);
            GameManager.instance.buttonInteract.SetActive(false);
            GameManager.instance.buttonInfo.text = null;
        }
    }
}
