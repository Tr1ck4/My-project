using UnityEngine;
using System;

[System.Serializable]
public class WeaponData
{
    public string weaponName;
    public float damage;
    public int Mag;
    public int maxMag;
    public int Ammo;
    public float ShootSpeed;
    public float range;
    public float price;
    public float updateCost; 
    public GameObject weaponPrefab; // Reference to prefab
    public Sprite weaponImage; // Reference to image

    public bool isSold = false;

    // Default constructor
    public WeaponData() { }

    // Copy constructor
    public WeaponData(WeaponData other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        weaponName = other.weaponName;
        damage = other.damage;
        Mag = other.Mag;
        maxMag = other.maxMag;
        Ammo = other.Ammo;
        ShootSpeed = other.ShootSpeed;
        range = other.range;
        price = other.price;
        updateCost = other.updateCost;
        weaponPrefab = other.weaponPrefab;
        weaponImage = other.weaponImage;
        isSold = other.isSold;
    }
}
