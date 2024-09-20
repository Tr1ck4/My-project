using UnityEngine;

public class ZombieRenderingController : MonoBehaviour
{
    private Animator animator;
    private Zombie zombieScript;  // Your AI script or movement logic
    private Renderer[] renderers;
    private AudioSource audioSource;
    private bool isDisabled = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        zombieScript = GetComponent<Zombie>();
        audioSource = GetComponent<AudioSource>();
        renderers = GetComponentsInChildren<Renderer>();

        if (animator == null )
        {
            Debug.Log("Cannot find animator");
        }

        if (zombieScript == null)
        {
            Debug.Log("Cannot find zombieScript");
        }

        if (audioSource == null)
        {
            Debug.Log("Cannot find audioSource");
        }

        if (renderers == null)
        {
            Debug.Log("Cannot find renderers");
        }
    }

    public void DisableZombie()
    {
        if (zombieScript != null) zombieScript.enabled = false;
        if (animator != null) animator.enabled = false;

        // Disable rendering
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        isDisabled = true;
    }

    public void EnableZombie()
    {
        if (zombieScript != null) zombieScript.enabled = true;
        if (animator != null) animator.enabled = true;

        // Enable rendering
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
        }

        isDisabled = false;
    }

    public bool IsDisabled()
    {
        return isDisabled;
    }
}
