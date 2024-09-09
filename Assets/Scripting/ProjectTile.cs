using UnityEngine;

public class ProjectTile : MonoBehaviour
{

    public float damage;
    public float resist;
    public GameObject bulletPrefab;
    void OnTriggerEnter(Collider other)
    {
        if (other != null && other.name != "RigidBodyFPSController")
        {
            Object obj = other.gameObject.GetComponent<Object>();
            if (obj != null)
            {
                //obj.TakeDamage(damage, this.transform.position, resist);
            }
            Destroy(gameObject);
        }
    }

}
