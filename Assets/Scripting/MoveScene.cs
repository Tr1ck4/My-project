using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveScene : MonoBehaviour
{
    public void ToggleScene2(){
        PlayerData playerData = GameObject.Find("GameController").GetComponent<PlayerData>();
        if (playerData.inventory.Count > 0 ) SceneManager.LoadScene("FinalBossLevel");
    }
    public void ToggleScene1(){
        PlayerData playerData = GameObject.Find("GameController").GetComponent<PlayerData>();
        if (playerData.inventory.Count > 0 ) SceneManager.LoadScene("level 0");
    }

}
