using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth = 100;

    public void IncreaseHealth(int amount)
    {
        currentHealth += amount;
        Debug.Log("Health increased. Current health: " + currentHealth);
    }
}
