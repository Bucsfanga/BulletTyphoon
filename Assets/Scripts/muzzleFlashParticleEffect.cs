using UnityEngine;

public class muzzleFlashParticleEffect : MonoBehaviour
{
    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem[] muzzleFlashParticleEffects;

    private void Update()
    {
        if (Input.GetButtonDown("Shoot"))
        {
            PlayMuzzleFlash();
        }
    }

    private void PlayMuzzleFlash()
    {
        foreach (ParticleSystem particleEffect in muzzleFlashParticleEffects)
        {
            if (particleEffect != null)
            {
                particleEffect.Play();
            }
        }
    }
}
