using System.Collections.Generic;
using UnityEngine;

public class Player : Object
{
    public List<WeaponData> inventory;
    public int currentWeaponIndex = 0;
    public Transform mainCameraTransform;
    public WeaponDatabase weaponDB;

    void Start()
    {
        mainCameraTransform = Camera.main.transform;
        if (inventory.Count < 2) {
            inventory.Add(weaponDB.weaponList[0]);
        }
        EquipWeapon(inventory[currentWeaponIndex]);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchWeapon();
        }
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

    public void AddAmmo(int amount)
    {
        if (inventory.Count > 0)
        {
            inventory[currentWeaponIndex].Ammo += amount;
        }
    }

    public void AddWeapon(WeaponData newWeapon)
    {
        if (!inventory.Contains(newWeapon) && inventory.Count < 2)
        {
            inventory.Add(newWeapon);
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
}
