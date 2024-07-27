using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectTile : MonoBehaviour
{
    public float speed = 700.0f;
    public GameObject bullet;
    void SpawnTile(Vector3 pos){
        GameObject tile = Instantiate(bullet,pos);
        tile.GetComponent<Rigidbody>().AddRelativeForce(new Vector3 
                                                (0, speed,0));
    }
}
