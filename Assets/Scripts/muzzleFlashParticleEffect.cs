using UnityEngine;

public class muzzleFlashParticleEffect : MonoBehaviour
{
    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem[] muzzleFlashParticleEffects;
    [SerializeField] private playerController playerScript;


    private void Start()
    {
        if (playerScript ==null && GameManager.instance !=null)
        {
            playerScript = GameManager.instance.playerScript;
        }
    }

    private void Update()
    {
        // Check if game is not paused, player has ammo, and shoot button is pressed
        if (GameManager.instance != null &&!GameManager.instance.isPaused &playerScript != null && playerScript.currentAmmo > 0 &&
            Input.GetButton("Shoot"))
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
