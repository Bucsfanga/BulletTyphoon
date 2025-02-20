using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityType", menuName = "Scriptable Objects/AbilityType")]
public class AbilityType : ScriptableObject
{
    public enum Ability {
        Speed,
        Jump,
        Damage,
        GodMode,
        Points
    }
}
