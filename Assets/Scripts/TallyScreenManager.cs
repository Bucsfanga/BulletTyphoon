using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
public class TallyScreenManager : MonoBehaviour
{
    public GameObject tallyScreen;
    public TextMeshProUGUI completionTimeText;
    public TextMeshProUGUI damageTakenText;
    public TextMeshProUGUI stepsTakenText;
    public Button skipButton;
    public GameObject winMenu;

    private float completionTime;
    private int damageTaken;
    private int stepsTaken;
    private bool skipRequested = false;

    void Start()
    {
        tallyScreen.SetActive(false);
        skipButton.onClick.AddListener(SkipTally);
    }

    public void ShowTallyScreen(float time, int damage, int steps)
    {
        completionTime = time;
        damageTaken = damage;
        stepsTaken = steps;

        tallyScreen.SetActive(true);
        StartCoroutine(AnimateTally());
    }

    private IEnumerator AnimateTally()
    {
        float displayTime = 0;
        int displayDamage = 0;
        int displaySteps = 0;

        while (displayTime < completionTime || displayDamage < damageTaken || displaySteps < stepsTaken)
        {
            if (skipRequested)
            {
                displayTime = completionTime;
                displayDamage = damageTaken;
                displaySteps = stepsTaken;
                break;
            }

            displayTime = Mathf.Min(displayTime + Time.deltaTime * 5, completionTime);
            displayDamage = Mathf.Min(displayDamage + 1, damageTaken);
            displaySteps = Mathf.Min(displaySteps + 1, stepsTaken);

            completionTimeText.text = $"Completion Time: {displayTime:F1} sec";
            damageTakenText.text = $"Damage Taken: {displayDamage}";
            stepsTakenText.text = $"Steps Taken: {displaySteps}";

            yield return null;
        }

        completionTimeText.text = $"Completion Time: {completionTime:F1} sec";
        damageTakenText.text = $"Damage Taken: {damageTaken}";
        stepsTakenText.text = $"Steps Taken: {stepsTaken}";
    }

    public void SkipTally()
    {
        skipRequested = true;
    }

    public void ConfirmTally()
    {
        tallyScreen.SetActive(false); // Close tally screen
        winMenu.SetActive(true); // Now show Win Menu
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Press Space to skip
        {
            SkipTally();
        }
        if (Input.GetKeyDown(KeyCode.Return)) // Press "Enter" to continue
        {
            ConfirmTally();
        }
    }
}
