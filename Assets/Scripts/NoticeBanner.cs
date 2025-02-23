using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoticeBanner : MonoBehaviour
{
    public string textValue;
    public string defaultText = " ";
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
            case 2:
                textValue = "Press ENTER key to continue ...";
                _noticeBanner.text = textValue;
                break;
            default:
                break;

        }
        
    }

    IEnumerator FloodWarning()
    {
       // _noticeBanner.enabled = true;
        textValue = "Warning Flooding in 1o seconds";
        _noticeBanner.text = textValue;
        yield return new WaitForSeconds(0.5f);
        _noticeBanner.text = defaultText;
    }
    IEnumerator ClassifiedCollected()
    {
        textValue = "Secret document collected ...";
        _noticeBanner.text = textValue;
        yield return new WaitForSeconds(0.5f);
        _noticeBanner.text = defaultText;
    }
}
