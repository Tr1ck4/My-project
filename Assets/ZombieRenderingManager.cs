using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieRenderingManager : MonoBehaviour
{
    public GameObject player;
    public float deactivateDistance = 50f;
    public float checkInterval = 3f;

    void Start()
    {
        
        player = GameObject.Find("Player");
        StartCoroutine(CheckZombieDistances());
    }

    IEnumerator CheckZombieDistances()
    {
        while (true)
        {
            GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
            // GameObject[] zombieSpawners = GameObject.FindGameObjectsWithTag("ZombieSpawner");
            foreach (GameObject zombie in zombies)
            {
                if (zombie != null)
                {
                    float distanceToPlayer = Vector3.Distance(zombie.transform.position, player.transform.position);
                    
                    ZombieRenderingController zombieController = zombie.GetComponent<ZombieRenderingController>();

                    if (zombieController != null)
                    {
                        if (distanceToPlayer > deactivateDistance && !zombieController.IsDisabled())
                        {
                            Debug.Log("zombie is far, disabling");
                            // Disable components of the zombie
                            zombieController.DisableZombie();
                        }
                        else if (distanceToPlayer <= deactivateDistance && zombieController.IsDisabled())
                        {
                            // Re-enable components of the zombie
                            Debug.Log("zombie is near, enabling");
                            zombieController.EnableZombie();
                        }
                    }
                }

                //foreach (GameObject zombieSpawner in zombieSpawners)
                //{
                //    if (zombieSpawner != null)
                //    {
                //        float distanceToPlayer = Vector3.Distance(zombieSpawner.transform.position, player.transform.position);

                //        if (distanceToPlayer > deactivateDistance && zombieSpawner.activeInHierarchy)
                //        {
                //            zombieSpawner.SetActive(false);
                //        }
                //        else if (distanceToPlayer <= deactivateDistance && !zombieSpawner.activeInHierarchy)
                //        {
                //            zombieSpawner.SetActive(true);
                //        }
                //    }
                //}

                // Wait for the next interval
                yield return new WaitForSeconds(checkInterval);
            }
        }
    }
}
