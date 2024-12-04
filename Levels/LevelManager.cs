using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public SpriteRenderer currentBackgroundImage;
    public LevelData[] levels;
    private LevelData currentLevel;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length) {
            Debug.LogError("Invalid level index!");
            return;
        }

        currentLevel = levels[levelIndex];
        ApplyLevelData();

        // Log the selected difficulty
        Debug.Log($"Loading level {currentLevel.levelName} at {GameManager.Instance.currentDifficulty} difficulty.");

        GameManager.Instance.currentLevel = currentLevel;

        GameManager.Instance.SetGameState(GameManager.GameState.Game);
        SceneManager.LoadScene("Level");
    }


    private void ApplyLevelData()
    {
        if (currentBackgroundImage != null) {
            currentBackgroundImage.sprite = currentLevel.backgroundImage;
        }
        
        Debug.Log($"Loaded {currentLevel.levelName} with {currentLevel.enemiesCount} enemies.");
    }

    public LevelData GetCurrentLevelData()
    {
        return currentLevel;
    }

    public void CompleteLevel()
    {
        SaveManager.Instance.UnlockNextDifficulty(currentLevel.levelIndex, GameManager.Instance.currentDifficulty);
        Debug.Log($"Level {currentLevel.levelIndex} completed on {GameManager.Instance.currentDifficulty}. Next difficulty unlocked!");
    }
}
