using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class Object : MonoBehaviour
{
    public float Health;
    public float Speed;
    public float Ammor;
    public float Damage;
    public Rigidbody body;

    // Array or List of blood splatter prefabs
    public GameObject[] bloodSplatterPrefabs;

    // Time before the blood splatter disappears
    public float splatterLifetime = 5f;

    public virtual void Move(){
        Debug.Log("Object moving");
    }
    public void TakeDamage(float amount){
        Health -= amount*(1-Ammor/100);

        OnShot(transform.position, transform.rotation);

        //if (gameObject.tag == "Player")
        //{
        //    Debug.Log("Player HP: " + gameObject.GetComponent<Player>().Health);
        //}
        if (Health <= 0f){
            Die();
        }
    }

    public virtual void OnShot(Vector3 hitPosition, Quaternion hitRotation)
    {
        // Select a random blood splatter decal
        GameObject bloodSplatter = bloodSplatterPrefabs[Random.Range(0, bloodSplatterPrefabs.Length)];

        // Instantiate the blood splatter at the hit position
        GameObject instantiatedSplatter = Instantiate(bloodSplatter, hitPosition, hitRotation);

        // Start coroutine to destroy the splatter after the set lifetime
        StartCoroutine(RemoveSplatterAfterTime(instantiatedSplatter, splatterLifetime));
    }

    // Coroutine to destroy the blood splatter after a set time
    IEnumerator RemoveSplatterAfterTime(GameObject splatter, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(splatter);
    }
    public virtual void Die(){
        if (gameObject.tag == "Player"){
            // Assuming RigidbodyFirstPersonController is attached to the player object
            RigidbodyFirstPersonController controller = GameObject.Find("Player").GetComponent<RigidbodyFirstPersonController>();
            controller.Die();
            SceneManager.LoadScene("MainScreen");
        }
        else{
            Destroy(gameObject);
        }
    }
}
