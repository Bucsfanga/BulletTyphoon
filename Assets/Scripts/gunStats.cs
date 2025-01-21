using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject model;
    public int shootDamage;
    public int shootDist;
    public float shootRate;
    public int ammoCur, ammoMax;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    public float shootSoundVol;
}
