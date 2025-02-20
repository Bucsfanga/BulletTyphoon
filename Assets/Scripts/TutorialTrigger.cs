using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [TextArea] public string customTutorialText;
    [SerializeField] public Sprite customTutorialImage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.ShowTutorial(customTutorialText, customTutorialImage);
            gameObject.SetActive(false); // Turn off trigger after activation
        }
    }
}
