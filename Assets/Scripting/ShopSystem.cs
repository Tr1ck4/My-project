using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSystem : MonoBehaviour
{
    public WeaponDatabase weaponDatabase; 
    public GameObject weaponPrefab;       
    public Transform weaponParent;         
    public ScrollRect scrollRect;          
    public Button previousButton;          
    public Button nextButton;              

    private int currentPageIndex = 0;      
    private int totalPages;                

    void Start()
    {
        if (weaponDatabase == null || weaponPrefab == null || weaponParent == null || scrollRect == null || previousButton == null || nextButton == null)
        {
            Debug.LogError("Missing references. Ensure all fields are assigned in the Inspector.");
            return;
        }

        totalPages = Mathf.CeilToInt((float)weaponDatabase.weaponList.Count / 1);

        PopulateCarousel();

        previousButton.onClick.AddListener(ShowPreviousPage);
        nextButton.onClick.AddListener(ShowNextPage);
    }

    void PopulateCarousel()
    {

        foreach (Transform child in weaponParent)
        {
            Destroy(child.gameObject);
        }

        int startIndex = currentPageIndex; 
        int endIndex = Mathf.Min(startIndex + 1, weaponDatabase.weaponList.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            WeaponData weaponData = weaponDatabase.weaponList[i];

            GameObject weaponInstance = Instantiate(weaponPrefab, weaponParent);

            var damageText = weaponInstance.transform.Find("Damage")?.GetComponent<TMP_Text>();
            var maxMagText = weaponInstance.transform.Find("MaxMag")?.GetComponent<TMP_Text>();
            var ammoText = weaponInstance.transform.Find("Ammo")?.GetComponent<TMP_Text>();
            var firerateText = weaponInstance.transform.Find("Firerate")?.GetComponent<TMP_Text>();
            var rangeText = weaponInstance.transform.Find("Range")?.GetComponent<TMP_Text>();

            if (damageText != null) damageText.text = "Damage: " + weaponData.damage;
            if (maxMagText != null) maxMagText.text = "Max Mag: " + weaponData.maxMag;
            if (ammoText != null) ammoText.text = "Ammo: " + weaponData.Ammo;
            if (firerateText != null) firerateText.text = "Fire Rate: " + weaponData.ShootSpeed;
            if (rangeText != null) rangeText.text = "Range: " + weaponData.range;

            var UdamageButton = weaponInstance.transform.Find("UDamage")?.GetComponent<Button>();
            var UdamageText = UdamageButton?.GetComponentInChildren<TMP_Text>();
            var UmaxMagButton = weaponInstance.transform.Find("UMaxMag")?.GetComponent<Button>();
            var UmaxMagText = UmaxMagButton?.GetComponentInChildren<TMP_Text>();
            var UammoButton = weaponInstance.transform.Find("UAmmo")?.GetComponent<Button>();
            var UammoText = UammoButton?.GetComponentInChildren<TMP_Text>();
            var UfirerateButton = weaponInstance.transform.Find("UFirerate")?.GetComponent<Button>();
            var UfirerateText = UfirerateButton?.GetComponentInChildren<TMP_Text>();
            var UrangeButton = weaponInstance.transform.Find("URange")?.GetComponent<Button>();
            var UrangeText = UrangeButton?.GetComponentInChildren<TMP_Text>();

            if (UdamageText != null) UdamageText.text = (1.5 * weaponData.updateCost).ToString();
            if (UmaxMagText != null) UmaxMagText.text = (1 * weaponData.updateCost).ToString();
            if (UammoText != null) UammoText.text = (1 * weaponData.updateCost).ToString();
            if (UfirerateText != null) UfirerateText.text = (1.75 * weaponData.updateCost).ToString();
            if (UrangeText != null) UrangeText.text = (0.75 * weaponData.updateCost).ToString();

            var weaponImage = weaponInstance.transform.Find("Image")?.GetComponent<Image>();
            if (weaponImage != null) weaponImage.sprite = weaponData.weaponImage;

            var weaponName = weaponInstance.transform.Find("WeaponName")?.GetComponent<TMP_Text>();
            if (weaponName != null) weaponName.text = weaponData.weaponName;
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
        }
    }

    void ShowNextPage()
    {
        if (currentPageIndex < totalPages - 1)
        {
            currentPageIndex++;
            PopulateCarousel();
        }
    }
}
