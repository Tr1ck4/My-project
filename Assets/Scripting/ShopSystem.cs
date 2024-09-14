using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ShopSystem : MonoBehaviour
{
    public static ShopSystem instance { get; private set; }
    private WeaponDatabase database;
    public GameController gameController;
    public GameObject weaponPrefab;       
    public Transform weaponParent;         
    public ScrollRect scrollRect;          
    public Button previousButton;          
    public Button nextButton; 
    public TMP_Text moneyText;            

    private int currentPageIndex = 0;      
    private int totalPages; 

    void Awake(){
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
        database = gameController.weaponDatabase;
        totalPages = Mathf.CeilToInt((float)database.weaponList.Count / 1);

        PopulateCarousel();

        previousButton.onClick.AddListener(ShowPreviousPage);
        nextButton.onClick.AddListener(ShowNextPage);

        UpdateButtonInteractability();
    }

    void Update(){
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        database = gameController.weaponDatabase;
        moneyText.text = "$" + gameController.money.ToString();
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
                upgradeHandler.Initialize(weaponData, this);
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
        if (gameController.money >= amount){
            gameController.money -= amount;
        }
    }
}
