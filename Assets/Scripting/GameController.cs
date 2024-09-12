using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject toggle;
    public ShopSystem system;
    public PlayerData playerData;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)){
           
            toggle.gameObject.SetActive(!toggle.activeInHierarchy);
        }
    }

    void OnApplicationQuit(){
        system.SaveData();
    }
}
