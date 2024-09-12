using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class WeaponDatabaseWrapper
{
    public List<WeaponData> weaponList;
    public float money;
}



public class ShopSystem : MonoBehaviour
{
    private ShopSystem instance;
    public WeaponDatabase weaponDatabase; 
    private WeaponDatabase database;
    public GameObject weaponPrefab;       
    public Transform weaponParent;         
    public ScrollRect scrollRect;          
    public Button previousButton;          
    public Button nextButton; 
    public TMP_Text moneyText; 
    public float money;            

    private int currentPageIndex = 0;      
    private int totalPages; 

    void Awake(){
        if( instance == null ){
            instance = this.instance;
            DontDestroyOnLoad(this.gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }               

    void Start()
    {
        LoadData();
        totalPages = Mathf.CeilToInt((float)database.weaponList.Count / 1);

        PopulateCarousel();

        previousButton.onClick.AddListener(ShowPreviousPage);
        nextButton.onClick.AddListener(ShowNextPage);

        UpdateButtonInteractability();
    }

    void Update(){
        moneyText.text = "$" + money.ToString();
    }

     void PopulateCarousel()
    {
        foreach (Transform child in weaponParent)
        {
            Destroy(child.gameObject);
        }

        int startIndex = currentPageIndex; 
        int endIndex = Mathf.Min(startIndex + 1, database.weaponList.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            WeaponData weaponData = database.weaponList[i];

            GameObject weaponInstance = Instantiate(weaponPrefab, weaponParent);

            WeaponUpgradeHandler upgradeHandler = weaponInstance.GetComponent<WeaponUpgradeHandler>();
            if (upgradeHandler != null)
            {
                upgradeHandler.Initialize(weaponData,this);
            }
        }

        float pageWidth = scrollRect.viewport.rect.width;
        scrollRect.horizontalNormalizedPosition = currentPageIndex / (float)(totalPages - 1);
    }

    void ShowPreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            PopulateCarousel();
            UpdateButtonInteractability();
        }
    }

    void ShowNextPage()
    {
        if (currentPageIndex < totalPages - 1)
        {
            currentPageIndex++;
            PopulateCarousel();
            UpdateButtonInteractability();
        }
    }

    void UpdateButtonInteractability()
    {
        previousButton.interactable = currentPageIndex > 0;
        nextButton.interactable = currentPageIndex < totalPages - 1;
    }

    public void DeductMoney(float amount){
        if (money >= amount){
            money -= amount;
        }
    }

    public void SaveData()
    {
        if (database != null)
        {
            WeaponDatabaseWrapper databaseWrapper = new WeaponDatabaseWrapper { weaponList = database.weaponList, money = this.money };
            string json = JsonUtility.ToJson(databaseWrapper);
            System.IO.File.WriteAllText("gunData.json", json);
        }
        else
        {
            WeaponDatabaseWrapper databaseWrapper = new WeaponDatabaseWrapper { weaponList = weaponDatabase.weaponList, money = this.money };
            string json = JsonUtility.ToJson(databaseWrapper);
            System.IO.File.WriteAllText("gunData.json", json);
        }
    }


    public void LoadData()
    {
        string json = System.IO.File.ReadAllText("gunData.json");
        WeaponDatabaseWrapper databaseWrapper = JsonUtility.FromJson<WeaponDatabaseWrapper>(json);

        database = ScriptableObject.CreateInstance<WeaponDatabase>();
        database.weaponList = databaseWrapper.weaponList;
        this.money = databaseWrapper.money;
    }

    public void FreshData()
    {   
        database = ScriptableObject.CreateInstance<WeaponDatabase>();
        database.weaponList = weaponDatabase.weaponList;
        this.money = 200;
        SaveData();
    }



}
