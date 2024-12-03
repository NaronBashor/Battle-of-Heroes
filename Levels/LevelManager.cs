using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public SpriteRenderer currentBackgroundImage;
    public LevelData[] levels;
    private LevelData currentLevel;

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length) {
            Debug.LogError("Invalid level index!");
            return;
        }

        currentLevel = levels[levelIndex];
        ApplyLevelData();
    }

    private void ApplyLevelData()
    {
        // Example: Change background color based on level
        currentBackgroundImage.sprite = currentLevel.backgroundImage;

        // Example: Spawn enemies or configure the level
        Debug.Log($"Loading {currentLevel.levelName} with {currentLevel.enemiesCount} enemies and a max index of {currentLevel.maxEnemyIndex}.");
    }

    public LevelData GetCurrentLevelData()
    {
        return currentLevel;
    }
}
