using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ZombieDatabase", menuName = "Databases/ZombieDatabase")]
public class ZombieDatabase : ScriptableObject
{
    public List<ZombieData> zombieList;
}
