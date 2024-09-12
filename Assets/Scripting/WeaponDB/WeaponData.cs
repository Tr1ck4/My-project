using UnityEngine;
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
    public GameObject weaponPrefab;
    public Sprite weaponImage;

    public bool isSold = false;
}
