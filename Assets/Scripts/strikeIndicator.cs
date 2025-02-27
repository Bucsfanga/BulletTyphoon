using UnityEngine;

public class strikeIndicator : MonoBehaviour
{
    private string playerTag;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTag = GameManager.instance.player.tag;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            GameManager.instance.populateBanner(3);
        }
    }
}
