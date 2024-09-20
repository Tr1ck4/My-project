using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class ZombieCheck : MonoBehaviour
{
    public string mainSceneName = "MainScreen"; 
    public float checkInterval = 2f; 

    void Start()
    {
        StartCoroutine(CheckForMutantPeriodically());
    }

    //IEnumerator CheckForZombiesPeriodically()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(checkInterval);

    //        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
    //        if (zombies.Length == 0)
    //        {
    //            RigidbodyFirstPersonController controller = GameObject.Find("Player").GetComponent<RigidbodyFirstPersonController>();
    //            controller.Die();
    //            // Load the main scene
    //            SceneManager.LoadScene(mainSceneName);
    //            yield break;
    //        }
    //    }
    //}

    IEnumerator CheckForMutantPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            GameObject mutant = GameObject.Find("Mutant");
            if (mutant == null)
            {
                RigidbodyFirstPersonController controller = GameObject.Find("Player").GetComponent<RigidbodyFirstPersonController>();
                controller.Die();
                // Load the main scene
                SceneManager.LoadScene(mainSceneName);
                yield break;
            }
        }
    }
}
