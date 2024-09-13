using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveScene : MonoBehaviour
{
    public void ToggleScene2(){
        SceneManager.LoadScene("FinalBossLevel");
    }
    public void ToggleScene1(){
        SceneManager.LoadScene("level 0");
    }

}
