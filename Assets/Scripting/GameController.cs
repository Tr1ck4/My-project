using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WeaponDatabaseWrapper
{
    public List<WeaponData> weaponList;
    public float money;
}

public class GameController : MonoBehaviour
{
    private HomeController homeController;
    private Button shopToggle;
    private GameObject shopDisplay;
    private GameController instance;
    private GameObject toggle;
    public WeaponDatabase weaponDatabase;
    public WeaponDatabase originData;
    public PlayerData playerData;
    public int status;
    public float money;
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
        homeController = GameObject.Find("HomeController").GetComponent<HomeController>();
        if( homeController!= null){
            if (homeController.status == 0){
                FreshData();
            }
            if (homeController.status == 1){
                LoadData();
            }
        }
    }
    void Update()
    {
        if (toggle == null){toggle = GameObject.Find("ToggleTutorial");}
        if (Input.GetKeyDown(KeyCode.M)){
            toggle.gameObject.SetActive(!toggle.activeInHierarchy);
        }
    }
    void OnApplicationQuit(){
        SaveData();
    }

    public void SaveData()
    {
        WeaponDatabaseWrapper databaseWrapper = new WeaponDatabaseWrapper { weaponList = weaponDatabase.weaponList, money = this.money };
        string json = JsonUtility.ToJson(databaseWrapper);
        System.IO.File.WriteAllText("gunData.json", json);
    }


    void LoadData()
    {
        string json = System.IO.File.ReadAllText("gunData.json");
        WeaponDatabaseWrapper databaseWrapper = JsonUtility.FromJson<WeaponDatabaseWrapper>(json);

        weaponDatabase = ScriptableObject.CreateInstance<WeaponDatabase>();
        weaponDatabase.weaponList = databaseWrapper.weaponList;
        this.money = databaseWrapper.money;
    }

    void FreshData()
    {   
        weaponDatabase = ScriptableObject.CreateInstance<WeaponDatabase>();
        weaponDatabase.weaponList = originData.weaponList;
        this.money = 200;
        SaveData();
    }


}
