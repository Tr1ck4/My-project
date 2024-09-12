using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class mainscreen : MonoBehaviour
{
    private GameController gameController;
    private GameObject shopdisplay;
    private Button toggleButton;
    public Button newGame;
    public Button continueGame;
    public Button tutorialGame;
    public Button settingsGame;
    public Button exitGame;
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        shopdisplay = GameObject.Find("ShopSystem");
        newGame.onClick.AddListener(EnterGame);
        continueGame.onClick.AddListener(EnterContinue);
        tutorialGame.onClick.AddListener(EnterTutorial);
        settingsGame.onClick.AddListener(EnterSetting);
        exitGame.onClick.AddListener(ExitGame);
    }
    void EnterGame(){
        gameController.system.FreshData();
        SceneManager.LoadScene("MainScreen");
    }
    void EnterContinue(){
        gameController.system.LoadData();
        SceneManager.LoadScene("MainScreen");
    }
    void EnterTutorial(){
        
    }
    void EnterSetting(){

    }
    void ExitGame(){
        gameController.system.SaveData();
        Application.Quit();
    }
}
