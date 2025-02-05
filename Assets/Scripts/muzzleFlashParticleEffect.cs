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
        
    }

    public void PlayMuzzleFlash()
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
