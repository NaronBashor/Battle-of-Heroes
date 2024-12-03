using UnityEngine;
using System.IO;

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
        LoadGame();
    }

    public bool SaveExists()
    {
        return File.Exists(saveFilePath);
    }

    public void NewGame()
    {
        gameData = new GameData(charDatabase); // Reset using defaults
        SaveGame();
        //Debug.Log("Game data reset to default values.");
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
            gameData = JsonUtility.FromJson<GameData>(json);
            RefreshAllButtons();
            //Debug.Log("Game loaded!");
        } else {
            //Debug.Log("No save file found. Starting fresh.");
            SaveGame();
        }
    }

    public void RefreshAllButtons()
    {
        UnlockButton[] buttons = FindObjectsByType<UnlockButton>(FindObjectsSortMode.None);
        foreach (var button in buttons) {
            button.InitUnlockButton();
        }
    }
}
