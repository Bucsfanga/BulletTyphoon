using System.Diagnostics.Contracts;
using UnityEngine;

[CreateAssetMenu]
public class Item_Classified : ScriptableObject
{
    public GameObject model;
    private string level;

    public void CopyTo(Item_Classified target)
    {
        target.model = model;
        target.level = level;
    }

    public void SetLevel(string _level)
    {
        level = _level;
    }
}