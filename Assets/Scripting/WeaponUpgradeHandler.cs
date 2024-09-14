using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUpgradeHandler : MonoBehaviour
{
    private PlayerData playerData;
    public WeaponData weaponData;
    private ShopSystem shopSystem;
    public Image weaponImage;
    public TMP_Text weaponText;
    public TMP_Text damageText;
    public TMP_Text maxMagText;
    public TMP_Text ammoText;
    public TMP_Text firerateText;
    public TMP_Text rangeText;

    public TMP_Text weaponPrice;
    public Button weaponBuy;

    public Button UdamageButton;
    public TMP_Text UdamageText;
    public Button UmaxMagButton;
    public TMP_Text UmaxMagText;
    public Button UammoButton;
    public TMP_Text UammoText;
    public Button UfirerateButton;
    public TMP_Text UfirerateText;
    public Button UrangeButton;
    public TMP_Text UrangeText;

    private float damageUpgradeCost;
    private float maxMagUpgradeCost;
    private float ammoUpgradeCost;
    private float fireRateUpgradeCost;
    private float rangeUpgradeCost;

    private int maxUpgrades = 30;

    private int damageUpgradeCount;
    private int maxMagUpgradeCount;
    private int ammoUpgradeCount;
    private int fireRateUpgradeCount;
    private int rangeUpgradeCount;

    void Start()
    {
        playerData = GameObject.Find("GameController").GetComponent<PlayerData>();
        shopSystem = GameObject.Find("ShopSystem").GetComponent<ShopSystem>();

        damageUpgradeCount = PlayerPrefs.GetInt(weaponData.weaponName + "_damageUpgradeCount", 0);
        maxMagUpgradeCount = PlayerPrefs.GetInt(weaponData.weaponName + "_maxMagUpgradeCount", 0);
        ammoUpgradeCount = PlayerPrefs.GetInt(weaponData.weaponName + "_ammoUpgradeCount", 0);
        fireRateUpgradeCount = PlayerPrefs.GetInt(weaponData.weaponName + "_fireRateUpgradeCount", 0);
        rangeUpgradeCount = PlayerPrefs.GetInt(weaponData.weaponName + "_rangeUpgradeCount", 0);

        CheckButtonStates();
    }

    void Update(){
        CheckButtonStates();
    }

    public void Initialize(WeaponData weaponData, ShopSystem shopSystem)
    {
        this.weaponData = weaponData;
        this.shopSystem = shopSystem;

        weaponText.text = weaponData.weaponName;
        weaponImage.sprite = weaponData.weaponImage;
        if(weaponData.isSold){
            weaponPrice.text = "Equip";
        }
        else{
            weaponPrice.text = weaponData.price.ToString();
        }

        damageText.text = "Damage: " + weaponData.damage;
        maxMagText.text = "Max Mag: " + weaponData.maxMag;
        ammoText.text = "Ammo: " + weaponData.Ammo;
        firerateText.text = "Fire Rate: " + weaponData.ShootSpeed;
        rangeText.text = "Range: " + weaponData.range;

        damageUpgradeCost = 1.5f * weaponData.updateCost;
        maxMagUpgradeCost = 1 * weaponData.updateCost;
        ammoUpgradeCost = 1 * weaponData.updateCost;
        fireRateUpgradeCost = 1.75f * weaponData.updateCost;
        rangeUpgradeCost = 0.75f * weaponData.updateCost;

        UdamageText.text = damageUpgradeCost.ToString();
        UmaxMagText.text = maxMagUpgradeCost.ToString();
        UammoText.text = ammoUpgradeCost.ToString();
        UfirerateText.text = fireRateUpgradeCost.ToString();
        UrangeText.text = rangeUpgradeCost.ToString();

        UdamageButton.onClick.AddListener(UpgradeDamage);
        UmaxMagButton.onClick.AddListener(UpgradeMaxMag);
        UammoButton.onClick.AddListener(UpgradeAmmo);
        UfirerateButton.onClick.AddListener(UpgradeFireRate);
        UrangeButton.onClick.AddListener(UpgradeRange);
        weaponBuy.onClick.AddListener(weaponData.isSold? EquipGun : BuyGun);
        CheckButtonStates();
    }

    void CheckButtonStates()
    {
        UdamageButton.interactable = shopSystem.gameController.money >= damageUpgradeCost && damageUpgradeCount < maxUpgrades;
        UmaxMagButton.interactable = shopSystem.gameController.money >= maxMagUpgradeCost && maxMagUpgradeCount < maxUpgrades;
        UammoButton.interactable = shopSystem.gameController.money >= ammoUpgradeCost && ammoUpgradeCount < maxUpgrades;
        UfirerateButton.interactable = shopSystem.gameController.money >= fireRateUpgradeCost && fireRateUpgradeCount < maxUpgrades;
        UrangeButton.interactable = shopSystem.gameController.money >= rangeUpgradeCost && rangeUpgradeCount < maxUpgrades;
    }

    public void BuyGun()
    {
        if (shopSystem.gameController.money >= weaponData.price)
        {
            weaponPrice.text = "Equip";
            weaponData.isSold = true;
            shopSystem.DeductMoney((float)weaponData.price); 
            weaponBuy.onClick.RemoveAllListeners();
            weaponBuy.onClick.AddListener(EquipGun);
        }
    }

    public void EquipGun(){
        weaponPrice.text = "Equipped";
        playerData.AddWeapon(weaponData);
        weaponBuy.interactable = false;
    }

    public void UpgradeDamage()
    {
        if (damageUpgradeCount < maxUpgrades && shopSystem.gameController.money >= damageUpgradeCost)
        {
            weaponData.damage += 1;
            damageText.text = "Damage: " + weaponData.damage;
            shopSystem.DeductMoney((float)damageUpgradeCost);
            damageUpgradeCount++;

            PlayerPrefs.SetInt(weaponData.weaponName + "_damageUpgradeCount", damageUpgradeCount);

            CheckButtonStates();
        }
    }

    public void UpgradeMaxMag()
    {
        if (maxMagUpgradeCount < maxUpgrades && shopSystem.gameController.money >= maxMagUpgradeCost)
        {
            weaponData.maxMag += 1;
            maxMagText.text = "Max Mag: " + weaponData.maxMag;
            shopSystem.DeductMoney((float)maxMagUpgradeCost);
            maxMagUpgradeCount++;

            PlayerPrefs.SetInt(weaponData.weaponName + "_maxMagUpgradeCount", maxMagUpgradeCount);

            CheckButtonStates();
        }
    }

    public void UpgradeAmmo()
    {
        if (ammoUpgradeCount < maxUpgrades && shopSystem.gameController.money >= ammoUpgradeCost)
        {
            weaponData.Ammo += 2;
            ammoText.text = "Ammo: " + weaponData.Ammo;
            shopSystem.DeductMoney((float)ammoUpgradeCost);
            ammoUpgradeCount++;

            PlayerPrefs.SetInt(weaponData.weaponName + "_ammoUpgradeCount", ammoUpgradeCount);

            CheckButtonStates();
        }
    }

    public void UpgradeFireRate()
    {
        if (fireRateUpgradeCount < maxUpgrades && shopSystem.gameController.money >= fireRateUpgradeCost)
        {
            weaponData.ShootSpeed += 0.1f;
            firerateText.text = "Fire Rate: " + weaponData.ShootSpeed;
            shopSystem.DeductMoney((float)fireRateUpgradeCost);
            fireRateUpgradeCount++;

            PlayerPrefs.SetInt(weaponData.weaponName + "_fireRateUpgradeCount", fireRateUpgradeCount);

            CheckButtonStates();
        }
    }

    public void UpgradeRange()
    {
        if (rangeUpgradeCount < maxUpgrades && shopSystem.gameController.money >= rangeUpgradeCost)
        {
            weaponData.range += 2;
            rangeText.text = "Range: " + weaponData.range;
            shopSystem.DeductMoney((float)rangeUpgradeCost);
            rangeUpgradeCount++;

            PlayerPrefs.SetInt(weaponData.weaponName + "_rangeUpgradeCount", rangeUpgradeCount);

            CheckButtonStates();
        }
    }
}
