using UnityEngine;

public class MainController : MonoBehaviour
{
    private ShopSystem shopSystem;
    private Setting settingSystem;
    
    void Start()
    {
        shopSystem = ShopSystem.instance;
        settingSystem = Setting.instance;

        if (shopSystem != null)
        {
            shopSystem.gameObject.SetActive(false); // Or other initialization logic
        }
        if (settingSystem != null)
        {
            settingSystem.gameObject.SetActive(false); // Or other initialization logic
        }
    }

    public void Toggle1()
    {
        if (shopSystem != null)
        {
            shopSystem.gameObject.SetActive(!shopSystem.gameObject.activeSelf);
        }
    }

    public void Toggle2()
    {
        if (settingSystem != null)
        {
            settingSystem.gameObject.SetActive(!settingSystem.gameObject.activeSelf);
        }
    }
}
