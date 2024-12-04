using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Singleton Instance")]
    public static LevelManager Instance;

    [Header("Level Data")]
    [SerializeField] private LevelData[] levels;
    [SerializeField] private LevelData currentLevel;

    [Header("UI Components")]
    [SerializeField] private SpriteRenderer currentBackgroundImage;

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
        //Debug.Log($"Loading level {currentLevel.levelName} at {GameManager.Instance.currentDifficulty} difficulty.");

        GameManager.Instance.currentLevel = currentLevel;

        GameManager.Instance.SetGameState(GameManager.GameState.Game);
        SceneManager.LoadScene("Level");
    }


    private void ApplyLevelData()
    {
        if (currentBackgroundImage != null) {
            currentBackgroundImage.sprite = currentLevel.backgroundImage;
        }
        
        //Debug.Log($"Loaded {currentLevel.levelName} with {currentLevel.enemiesCount} enemies.");
    }

    public LevelData GetCurrentLevelData()
    {
        return currentLevel;
    }
    public void CompleteLevel()
    {
        LevelData thisCurrentLevel = GameManager.Instance.currentLevel;
        Difficulty thisDifficulty = GameManager.Instance.GetDifficulty();

        // Get the progress for the current level
        var currentLevelProgress = SaveManager.Instance.gameData.GetLevelProgress(thisCurrentLevel.levelIndex);
        if (currentLevelProgress == null) return;

        // Update the stars earned for the current level
        int starsForThisDifficulty = thisDifficulty switch {
            Difficulty.Easy => 1,
            Difficulty.Medium => 2,
            Difficulty.Hard => 3,
            _ => 0
        };
        currentLevelProgress.starsEarned = Mathf.Max(currentLevelProgress.starsEarned, starsForThisDifficulty);
        SaveManager.Instance.gameData.SetLevelProgress(thisCurrentLevel.levelIndex, currentLevelProgress);

        // Unlock the next difficulty for this level
        SaveManager.Instance.UnlockNextDifficulty(thisCurrentLevel.levelIndex, thisDifficulty);

        // Unlock the next level (if it exists and is locked)
        int nextLevelIndex = thisCurrentLevel.levelIndex + 1;
        var nextLevelProgress = SaveManager.Instance.gameData.GetLevelProgress(nextLevelIndex);
        if (nextLevelProgress != null && nextLevelProgress.starsEarned == 0) {
            nextLevelProgress.starsEarned = 1; // Unlock the next level with Easy difficulty
            SaveManager.Instance.gameData.SetLevelProgress(nextLevelIndex, nextLevelProgress);
        }

        // Save the updated progress
        SaveManager.Instance.SaveGame();
    }

}
