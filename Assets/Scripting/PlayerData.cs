using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public List<WeaponData> inventory = new List<WeaponData>(2); 

    public void AddWeapon(WeaponData newWeapon)
    {
        if (!inventory.Contains(newWeapon))
        {
            if (newWeapon.weaponName == "G17" || newWeapon.weaponName == "Desert Eagle" || newWeapon.weaponName == "G14")
            {
                if (inventory.Count > 0)
                {
                    inventory[0] = newWeapon;
                }
                else
                {
                    inventory.Add(newWeapon);
                }
            }
            else
            {
                if (inventory.Count > 1)
                {
                    inventory[1] = newWeapon;
                }
                else if (inventory.Count == 1)
                {
                    inventory.Add(newWeapon); 
                }
                else
                {
                    inventory.Add(newWeapon);
                }
            }
        }
        else
        {
            Debug.Log($"{newWeapon.weaponName} is already in the inventory.");
        }
    }
}
