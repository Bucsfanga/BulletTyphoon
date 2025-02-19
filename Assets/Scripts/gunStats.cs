using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject model;
    public int shootDamage;
    public int shootDist;
    public float shootRate;
    public int ammoCur, ammoMax, ammoTotal;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    public float shootSoundVol;

    public string gunID;

    // Function to deep copy gunStats
    public void CopyTo(gunStats target)
    {
        target.model = model;
        target.shootDamage = shootDamage;
        target.shootDist = shootDist;
        target.shootRate = shootRate;
        target.ammoCur = ammoCur;
        target.ammoMax = ammoMax;
        target.ammoTotal = ammoTotal;
        target.hitEffect = hitEffect;
        target.shootSound = shootSound;
        target.shootSoundVol = shootSoundVol;
        target.gunID = gunID;
    }
}
