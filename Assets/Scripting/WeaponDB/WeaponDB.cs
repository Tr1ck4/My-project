using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Databases/WeaponDatabase")]
public class WeaponDatabase : ScriptableObject
{
    public List<WeaponData> weaponList;
}
