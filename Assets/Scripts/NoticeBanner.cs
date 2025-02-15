using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoticeBanner : MonoBehaviour
{
    public string textValue;
    public TMP_Text _noticeBanner;

    // Notice is called to display text in the notice Banner area for the player
    public void Notice(int noticeCode)
    {
        switch (noticeCode)
        {
            case 0:
                StartCoroutine(FloodWarning());
                break;
            case 1:
                StartCoroutine(ClassifiedCollected());
                break;
            default:
                break;

        }
        
    }

    IEnumerator FloodWarning()
    {
        textValue = "Warning Flooding in 1o minutes";
        _noticeBanner.text = textValue;
        _noticeBanner.enabled = true;
        yield return new WaitForSeconds(0.5f);
        _noticeBanner.enabled = false;
    }
    IEnumerator ClassifiedCollected()
    {
        textValue = "Secret document collected ...";
        _noticeBanner.text = textValue;
        _noticeBanner.enabled = true;
        yield return new WaitForSeconds(0.5f);
        _noticeBanner.enabled = false;
    }
}
