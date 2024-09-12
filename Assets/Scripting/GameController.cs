using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private Button shopToggle;
    private GameObject shopDisplay;
    private GameController instance;
    private GameObject toggle;
    public ShopSystem system;
    public PlayerData playerData;
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
    void Start(){
        shopDisplay = GameObject.Find("ShopSystem");
        shopDisplay.SetActive(false);
    }
    void Update()
    {
        if (shopToggle == null){
            shopToggle = GameObject.Find("Shop")?.GetComponent<Button>();
        }
        if (shopToggle != null){
            shopToggle.onClick.AddListener(OnShopToggle);
        }
        if (toggle == null){toggle = GameObject.Find("ToggleTutorial");}
        if (system == null){system = GameObject.Find("ShopSystem")?.GetComponent<ShopSystem>();}
        if (Input.GetKeyDown(KeyCode.M)){
            toggle.gameObject.SetActive(!toggle.activeInHierarchy);
        }
    }
    void OnShopToggle(){
        shopDisplay.SetActive(!shopDisplay.activeSelf);
    }

    void OnApplicationQuit(){
        system.SaveData();
    }
}
