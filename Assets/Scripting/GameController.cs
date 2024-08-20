using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject toggle;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)){
           
            toggle.gameObject.SetActive(!toggle.activeInHierarchy);
        }
    }
}
