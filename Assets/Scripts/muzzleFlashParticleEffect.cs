using UnityEngine;

public class muzzleFlashParticleEffect : MonoBehaviour
{
    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem[] muzzleFlashParticleEffects;

    private void Update()
    {
        // Only play muzzle flash if the game is not paused and shoot button is pressed
        if (!GameManager.instance.isPaused && Input.GetButton("Shoot"))
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
