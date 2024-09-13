using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ZombieCheck : MonoBehaviour
{
    public string mainSceneName = "MainScreen"; 
    public float checkInterval = 2f; 

    void Start()
    {
        StartCoroutine(CheckForZombiesPeriodically());
    }

    IEnumerator CheckForZombiesPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
            if (zombies.Length == 0)
            {
                Debug.Log("No zombies found. Returning to the main scene.");

                // Load the main scene
                SceneManager.LoadScene(mainSceneName);
                yield break;
            }
        }
    }
}
