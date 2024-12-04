using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BarracksData", menuName = "Game/Barracks Data")]
public class BarracksData : ScriptableObject
{
    [Header("Base Settings")]
    [SerializeField] private string baseName; // Name of the base (optional)

    [Header("Barracks List")]
    [SerializeField] public List<Barracks> barracks = new List<Barracks>();

    public Sprite GetUpgradeSprite(int barrackIndex, int upgradeLevel)
    {
        if (barrackIndex < 0 || barrackIndex >= barracks.Count) return null;
        if (upgradeLevel < 0 || upgradeLevel >= barracks[barrackIndex].upgradeSprites.Length) return null;

        return barracks[barrackIndex].upgradeSprites[upgradeLevel];
    }

    public Sprite GetDamageSprite(int barrackIndex, int damageState)
    {
        if (barrackIndex < 0 || barrackIndex >= barracks.Count) return null;
        if (damageState < 0 || damageState >= barracks[barrackIndex].damageSprites.Length) return null;

        return barracks[barrackIndex].damageSprites[damageState];
    }
}

[System.Serializable]
public class Barracks
{
    public Sprite[] upgradeSprites = new Sprite[5]; // Sprites for upgrade levels 1-5
    public Sprite[] damageSprites = new Sprite[5]; // Sprites for damage states (5 states per level)
    public int health; // Base health for this barracks level
}

