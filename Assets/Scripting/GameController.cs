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
    private GameController instance;
    private Button shopToggle;
    private GameObject shopDisplay;
    private GameObject toggle;
    public WeaponDatabase weaponDatabase;
    public WeaponDatabase originData;
    public PlayerData playerData;
    public float money;
    public float Health;
    public float Ammor;
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


    public void LoadData()
    {
        string json = System.IO.File.ReadAllText("gunData.json");
        WeaponDatabaseWrapper databaseWrapper = JsonUtility.FromJson<WeaponDatabaseWrapper>(json);
        Debug.Log(databaseWrapper.weaponList);
        Debug.Log(databaseWrapper.money);
        weaponDatabase = ScriptableObject.CreateInstance<WeaponDatabase>();
        weaponDatabase.weaponList = databaseWrapper.weaponList;
        this.money = databaseWrapper.money;
    }

    public void FreshData()
    {
        PlayerPrefs.DeleteAll();
        weaponDatabase = ScriptableObject.CreateInstance<WeaponDatabase>();
        
        // Deep copy the weaponList
        weaponDatabase.weaponList = new List<WeaponData>();
        foreach (WeaponData weapon in originData.weaponList)
        {
            weaponDatabase.weaponList.Add(new WeaponData(weapon));
        }
        
        this.money = 200;
        
        SaveData();
    }



}
