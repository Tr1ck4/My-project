using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Setting : MonoBehaviour
{
    public float Health;
    public float Ammor;
    public float Money;
    
    public TMP_InputField healthInput;
    public TMP_InputField ammorInput;
    public TMP_InputField moneyInput;
    public Button saveButton;

    public static Setting instance { get; private set; }
    private GameController gameController;


    void Awake()
    {
        if( instance == null ){
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }

    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        Health = 100;
        Ammor = 0;
        Money = gameController.money;
        healthInput.onEndEdit.AddListener(delegate { ValidateHealth(); });
        ammorInput.onEndEdit.AddListener(delegate { ValidateAmmor(); });
        moneyInput.onEndEdit.AddListener(delegate { ValidateMoney(); });
        saveButton.onClick.AddListener(() => {
            Destroy(GameObject.Find("SettingModal"));
            Destroy(GameObject.Find("GameController"));
            SceneManager.LoadScene("HomeScreen");
        });
    }


    public void ValidateHealth()
    {
        if (float.TryParse(healthInput.text, out float result) && result > 0)
        {
            Health = result;
            gameController.Health = result; 
        }
        else
        {
            Debug.Log("Health value must be a number greater than 0.");
            healthInput.text = "";
        }
    }

    public void ValidateAmmor()
    {
        if (float.TryParse(ammorInput.text, out float result) && result > 0)
        {
            Ammor = result;
            gameController.Ammor = result;
        }
        else
        {
            Debug.Log("Ammor value must be a number greater than 0.");
            ammorInput.text = "";
        }
    }

    public void ValidateMoney()
    {
        if (float.TryParse(moneyInput.text, out float result) && result > 0)
        {
            Money = result;
            gameController.money = result;
        }
        else
        {
            Debug.Log("Money value must be a number greater than 0.");
            moneyInput.text = "";
        }
    }
}
