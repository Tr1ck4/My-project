using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class mainscreen : MonoBehaviour
{
    private GameController controller;
    private Button toggleButton;
    public Button newGame;
    public Button continueGame;
    public Button tutorialGame;
    public Button exitGame;
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("GameController").GetComponent<GameController>();
        newGame.onClick.AddListener(EnterGame);
        continueGame.onClick.AddListener(EnterContinue);
        tutorialGame.onClick.AddListener(EnterTutorial);
        exitGame.onClick.AddListener(ExitGame);
    }
    void EnterGame(){
        controller.FreshData();
        SceneManager.LoadScene("MainScreen");
    }
    void EnterContinue(){
        controller.LoadData();
        SceneManager.LoadScene("MainScreen");
    }
    void EnterTutorial(){
        
    }
    void ExitGame(){
        Application.Quit();
    }
}
