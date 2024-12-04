using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    [Header("Singleton Instance")]
    public static SaveManager Instance;

    [Header("Game Data")]
    [SerializeField] private CharacterDatabase charDatabase;
    [SerializeField] public GameData gameData;

    [Header("File Settings")]
    [SerializeField] private string saveFilePath;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Application.persistentDataPath + "/savefile.json";
        } else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (gameData == null) {
            Debug.LogWarning("Game data is null. Attempting to load or start a new game.");
            LoadGame();
        }
    }

    public bool SaveExists()
    {
        return File.Exists(saveFilePath);
    }

    public void NewGame()
    {
        gameData = new GameData(charDatabase); // Initialize `gameData` with defaults

        foreach (var entry in gameData.levelProgress) {
            entry.progress.starsEarned = 0; // Lock all levels
        }

        // Unlock the first level with no stars earned
        gameData.GetLevelProgress(0).starsEarned = -1; // Available state (unlocked)

        SaveGame();
    }


    public void DeleteSave()
    {
        if (SaveExists()) {
            File.Delete(saveFilePath);
            //Debug.Log("Save file deleted.");
        } else {
            Debug.LogWarning("No save file to delete.");
        }
        NewGame();
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(saveFilePath, json);
        RefreshAllButtons();
        //Debug.Log("Game saved!");
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath)) {
            string json = File.ReadAllText(saveFilePath);

            try {
                gameData = JsonUtility.FromJson<GameData>(json);
            } catch {
                Debug.LogError("Failed to deserialize save file. Starting a new game.");
                NewGame();
                return;
            }

            if (gameData == null) {
                Debug.LogWarning("Game data is null. Starting a new game.");
                NewGame();
                return;
            }

            // Ensure `levelProgress` is initialized
            if (gameData.levelProgress == null || gameData.levelProgress.Count == 0) {
                Debug.LogWarning("levelProgress is null or empty. Initializing.");
                gameData.ResetGameData(charDatabase);
            }

            RefreshAllButtons();
        } else {
            Debug.Log("Save file not found. Starting a new game.");
            NewGame();
        }
    }


    public void UnlockNextDifficulty(int levelIndex, Difficulty completedDifficulty)
    {
        var currentLevelProgress = gameData.GetLevelProgress(levelIndex);
        if (currentLevelProgress == null) return;

        // Unlock the next difficulty only
        if (completedDifficulty == Difficulty.Easy) {
            currentLevelProgress.mediumCompleted = true; // Unlock Medium
        } else if (completedDifficulty == Difficulty.Medium) {
            currentLevelProgress.hardCompleted = true; // Unlock Hard
        }

        // Save the updated progress
        gameData.SetLevelProgress(levelIndex, currentLevelProgress);
        SaveGame();

        Debug.Log($"UnlockNextDifficulty: Level {levelIndex}, Completed {completedDifficulty}");
        Debug.Log($"mediumCompleted = {currentLevelProgress.mediumCompleted}, hardCompleted = {currentLevelProgress.hardCompleted}");
    }






    public void UnlockNextLevel(int completedLevelIndex, int starsEarned)
    {
        // Get the progress for the completed level
        var completedLevelProgress = gameData.GetLevelProgress(completedLevelIndex);
        if (completedLevelProgress == null) return;

        // Update the stars for the completed level
        completedLevelProgress.starsEarned = Mathf.Clamp(starsEarned, 1, 3); // Clamp between 1 and 3
        gameData.SetLevelProgress(completedLevelIndex, completedLevelProgress);

        // Unlock the next level (if it exists)
        int nextLevelIndex = completedLevelIndex + 1;
        var nextLevelProgress = gameData.GetLevelProgress(nextLevelIndex);
        if (nextLevelProgress != null && nextLevelProgress.starsEarned == 0) {
            nextLevelProgress.starsEarned = 1; // Unlock the next level with Easy difficulty
            gameData.SetLevelProgress(nextLevelIndex, nextLevelProgress);
        }

        // Save the updated progress
        SaveGame();
    }




    public bool IsDifficultyUnlocked(int levelIndex, Difficulty difficulty)
    {
        var progress = gameData.GetLevelProgress(levelIndex);
        if (progress == null) return false;

        return difficulty switch {
            Difficulty.Easy => true, // Easy is always unlocked
            Difficulty.Medium => progress.mediumCompleted, // Medium requires Easy completion
            Difficulty.Hard => progress.hardCompleted, // Hard requires Medium completion
            _ => false
        };
    }






    public void RefreshAllButtons()
    {
        UnlockButton[] buttons = FindObjectsByType<UnlockButton>(FindObjectsSortMode.None);
        foreach (var button in buttons) {
            button.InitUnlockButton();
        }
    }
}
