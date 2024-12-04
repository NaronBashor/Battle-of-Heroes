using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelData")]
public class LevelData : ScriptableObject
{
    public int levelIndex;
    public string levelName;
    public int enemiesCount;
    public int maxEnemyIndex;
    public Sprite backgroundImage;
    public List<LevelDifficulty> difficulties = new List<LevelDifficulty>();
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

[System.Serializable]
public class LevelDifficulty
{
    public Difficulty difficulty; // Easy, Medium, Hard
    public float spawnRate;       // Rate at which enemies spawn
    public int maxEnemies;        // Maximum enemies for the wave
    public int passiveCoinAmount; // Passive coin income for this difficulty
}
