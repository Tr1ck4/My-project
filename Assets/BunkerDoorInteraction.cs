using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class BunkerDoor : MonoBehaviour
{
    public float activationRange = 6f;  // Distance from the door to display the message
    public GameObject player;  // Reference to the player object
    public GameObject floatingText;  // Reference to the 3D text object near the door
    private bool isPlayerNear = false;
    private string bunkerSceneName = "FinalBossLevel";

    void Start()
    {
        player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.Log("BunkerDoor: No player found");
        }

        floatingText = GameObject.Find("BunkerDoorInteractionMessage");
        if (floatingText == null)
        {
            Debug.Log("BunkerDoor: No BunkerDoorInteractionMessage found");
        }
        // Ensure the floating text is hidden at the start
        floatingText.SetActive(false);
    }

    void Update()
    {
        // Calculate distance between player and the door
        float distance = Vector3.Distance(player.transform.position, transform.position);

        // Check if the player is within range of the door
        if (distance <= activationRange)
        {
            ShowInteractionMessage();

            // Check if the player presses the 'E' key
            if (Input.GetKeyDown(KeyCode.E) && isPlayerNear)
            {
                EnterBunker();
            }
        }
        else
        {
            HideInteractionMessage();
        }
    }

    void ShowInteractionMessage()
    {
        floatingText.SetActive(true);  // Show the floating text
        isPlayerNear = true;
    }

    void HideInteractionMessage()
    {
        floatingText.SetActive(false);  // Hide the floating text
        isPlayerNear = false;
    }

    void EnterBunker()
    {
        // Logic for entering the bunker
        RigidbodyFirstPersonController controller = GameObject.Find("Player").GetComponent<RigidbodyFirstPersonController>();
        controller.Die();
        // Load the main scene
        SceneManager.LoadScene(bunkerSceneName);
        Debug.Log("Player entered the bunker!");
    }
}
