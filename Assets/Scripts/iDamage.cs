using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// interface for damageable objects
public interface IDamage
{
    // method to apply damage
    void takeDamage(int amount);
}