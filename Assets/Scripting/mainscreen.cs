using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class mainscreen : MonoBehaviour
{
    private HomeController controller;
    private Button toggleButton;
    public Button newGame;
    public Button continueGame;
    public Button tutorialGame;
    public Button exitGame;
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("HomeController").GetComponent<HomeController>();
        newGame.onClick.AddListener(EnterGame);
        continueGame.onClick.AddListener(EnterContinue);
        tutorialGame.onClick.AddListener(EnterTutorial);
        exitGame.onClick.AddListener(ExitGame);
    }
    void EnterGame(){
        controller.status = 0;
        SceneManager.LoadScene("MainScreen");
    }
    void EnterContinue(){
        controller.status = 1;
        SceneManager.LoadScene("MainScreen");
    }
    void EnterTutorial(){
        controller.status = 2;
    }
    void ExitGame(){
        Application.Quit();
    }
}
