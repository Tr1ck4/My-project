using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class Player : Object
{
    private Setting settings;
    public List<WeaponData> inventory;
    public int currentWeaponIndex = 0;
    public Transform mainCameraTransform;
    public WeaponDatabase weaponDB;
    public TMP_Text healthText;

    void Start()
    {
        settings = GameObject.Find("SettingModal").GetComponent<Setting>();
        this.Health = settings.Health;
        this.Ammor = settings.Ammor;
        inventory = GameObject.Find("GameController").GetComponent<PlayerData>().inventory;
        mainCameraTransform = Camera.main.transform;
        EquipWeapon(inventory[currentWeaponIndex]);
    }

    void Update()
    {
        inventory = GameObject.Find("GameController").GetComponent<PlayerData>().inventory;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchWeapon();
        }
        healthText.text = this.Health.ToString();
    }

    void SwitchWeapon()
    {
        if (inventory.Count > 1)
        {
            SaveCurrentWeapon();
            currentWeaponIndex = (currentWeaponIndex + 1) % inventory.Count;
            EquipWeapon(inventory[currentWeaponIndex]);
        }
    }

    void EquipWeapon(WeaponData weapon)
    {
        foreach (Transform child in mainCameraTransform)
        {
            Destroy(child.gameObject);
        }

        GameObject weaponInstance = Instantiate(weapon.weaponPrefab, mainCameraTransform);
        weaponInstance.transform.localPosition = Vector3.zero;
        weaponInstance.transform.localRotation = Quaternion.identity; 

        Gun gun = weaponInstance.GetComponent<Gun>();
        gun.Mag = weapon.Mag;
        gun.Ammo = weapon.Ammo;
    }

    public void AddAmmo()
    {
        if (inventory.Count > 0)
        {
            for(int i = 0; i < inventory.Count; i++){
                for(int j = 0 ; j < weaponDB.weaponList.Count; j++){
                    if (inventory[i].weaponName == weaponDB.weaponList[j].weaponName){
                        inventory[i].Ammo = weaponDB.weaponList[j].Ammo;
                        Gun gun = GameObject.FindObjectOfType<Gun>();
                        gun.Ammo = inventory[currentWeaponIndex].Ammo;
                        Debug.Log("Weapon " + weaponDB.weaponList[j].Ammo +  "Inventory" + inventory[i].Ammo);
                    }
                }
            }
        }
    }

    void SaveCurrentWeapon(){
        WeaponData currentWeapon = inventory[currentWeaponIndex];
        Gun currentGun = mainCameraTransform.GetComponentInChildren<Gun>();
        if (currentWeapon != null){
            currentWeapon.Mag = currentGun.Mag;
            currentWeapon.Ammo = currentGun.Ammo;
        }
    }
    void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "AmmoBox"){
            AddAmmo();
        }
    }

}
