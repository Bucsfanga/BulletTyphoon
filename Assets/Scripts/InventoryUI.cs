using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
public class InventoryUI : MonoBehaviour
{
    public GameObject itemPrefab; 
    public Transform contentPanel; 

    public void UpdateInventory(List<string> inventoryItems)
    {
        // Clear existing items
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // Populate the inventory list
        foreach (string item in inventoryItems)
        {
            GameObject newItem = Instantiate(itemPrefab, contentPanel);
            newItem.GetComponent<TextMeshProUGUI>().text = item;
        }
    }
}
