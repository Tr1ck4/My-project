using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public GameObject shopSystem;
    public GameObject settingSystem;
    
    void Start(){
        shopSystem.SetActive(false);
        settingSystem.SetActive(false);
    }

    public void Toggle1(){
        shopSystem.SetActive(!shopSystem.activeSelf);
    }

    public void Toggle2(){
        settingSystem.SetActive(!settingSystem.activeSelf);
    }
}
