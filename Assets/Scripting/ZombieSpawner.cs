using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnZombies());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [System.Serializable]
    public class ZombieType
    {
        public GameObject zombiePrefab;
        public int quantity;
    }

    public List<ZombieType> zombiesToSpawn;
    public float spawnInterval = 3.0f;
    public float spawnRadius = 10.0f;

    IEnumerator SpawnZombies()
    {
        foreach (var zombieType in zombiesToSpawn)
        {
            //Debug.Log("Spawning Zombie Type: " +  zombieType.ToString());
            for (int i = 0; i < zombieType.quantity; i++)
            {
                // Generate a random position within spawnRadius on the XZ plane to spawn Zombie
                Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
                Vector3 spawnPoint = new Vector3(transform.position.x + randomPos.x, transform.position.y, transform.position.z + randomPos.y);

                // Instantiate zombie
                Instantiate(zombieType.zombiePrefab, spawnPoint, Quaternion.identity);

                // Delay
                yield return new WaitForSeconds(spawnInterval);
            }
        }

    }

    // Draw the spawn radius in the Scene view
    private void OnDrawGizmosSelected()
    {
        // Set the Gizmo color
        Gizmos.color = Color.green;

        // Draw a 2D circle on the XZ plane representing the spawn radius
        DrawWireCircleXZ(transform.position, spawnRadius, 60);
    }

    // Helper method to draw a wire circle on the XZ plane
    private void DrawWireCircleXZ(Vector3 center, float radius, int numSegments)
    {
        float angleStep = 360f / numSegments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= numSegments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }

}
