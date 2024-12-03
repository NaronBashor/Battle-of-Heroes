using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelData")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public int enemiesCount;
    public int maxEnemyIndex;
    public Sprite backgroundImage; // Example of dynamic changes
    // Add other level-specific data as needed
}
