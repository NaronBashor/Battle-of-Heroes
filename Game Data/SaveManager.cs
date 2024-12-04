using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public CharacterDatabase charDatabase;
    public GameData gameData;

    private string saveFilePath;

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

        // Initialize `levelProgress`
        if (gameData.levelProgress == null) {
            gameData.levelProgress = new Dictionary<int, LevelProgress>();
        }

        for (int i = 0; i < 10; i++) // Assume 10 levels
        {
            gameData.levelProgress[i] = new LevelProgress();
        }

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
            if (gameData.levelProgress == null) {
                Debug.LogWarning("levelProgress is null. Initializing.");
                gameData.levelProgress = new Dictionary<int, LevelProgress>();
            }

            // Populate missing levels
            for (int i = 0; i < 10; i++) // Assume 10 levels
            {
                if (!gameData.levelProgress.ContainsKey(i)) {
                    gameData.levelProgress[i] = new LevelProgress();
                }
            }

            RefreshAllButtons();
        } else {
            Debug.Log("Save file not found. Starting a new game.");
            NewGame();
        }
    }





    public void UnlockNextDifficulty(int levelIndex, Difficulty completedDifficulty)
    {
        if (gameData.levelProgress == null) {
            Debug.LogWarning("levelProgress is null. Initializing.");
            gameData.levelProgress = new Dictionary<int, LevelProgress>();
        }

        if (!gameData.levelProgress.ContainsKey(levelIndex)) {
            gameData.levelProgress[levelIndex] = new LevelProgress();
        }

        var progress = gameData.levelProgress[levelIndex];
        if (completedDifficulty == Difficulty.Easy) progress.mediumCompleted = true;
        if (completedDifficulty == Difficulty.Medium) progress.hardCompleted = true;

        SaveGame();
    }


    public bool IsDifficultyUnlocked(int levelIndex, Difficulty difficulty)
    {
        if (!gameData.levelProgress.ContainsKey(levelIndex)) return false;

        var progress = gameData.levelProgress[levelIndex];
        return difficulty switch {
            Difficulty.Easy => true,
            Difficulty.Medium => progress.easyCompleted,
            Difficulty.Hard => progress.mediumCompleted,
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
