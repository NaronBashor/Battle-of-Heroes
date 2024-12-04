using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState
    {
        None,
        MainMenu,
        Shop,
        Heroes,
        Arena,
        Game,
        Map
    }

    public GameState state;
    public CharacterDatabase characterDatabase;
    private OptionsManager optionsManager;
    public Difficulty currentDifficulty; // Tracks the selected difficulty
    public LevelData currentLevel;      // Reference to the active level's data

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        SetGameState(GameState.MainMenu);
    }

    private void Start()
    {
        optionsManager = FindAnyObjectByType<OptionsManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (optionsManager.IsOptionsPanelOpen()) {
                optionsManager.CloseOptionsPanel();
            } else {
                optionsManager.OpenOptionsPanel();
            }
        }
    }

    public GameState GetCurrentGameState()
    {
        return state;
    }

    public void SetGameState(GameState newState)
    {
        if (newState != state) {
            state = newState;
        }
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;
    }

    public LevelDifficulty GetCurrentLevelDifficulty()
    {
        return currentLevel.difficulties.Find(d => d.difficulty == currentDifficulty);
    }

    public CharacterData[] GetEnemyPool(int levelIndex)
    {
        // Validate levelIndex
        if (levelIndex < 0 || levelIndex >= characterDatabase.enemyCharacters.Count) {
            Debug.LogError("Invalid level index!");
            return new CharacterData[0];
        }

        // Define enemy pool per level
        switch (levelIndex) {
            case 1:
                return new[] { characterDatabase.enemyCharacters[0] };
            case 2:
                return new[] { characterDatabase.enemyCharacters[0], characterDatabase.enemyCharacters[1] };
            case 3:
                return new[] { characterDatabase.enemyCharacters[0], characterDatabase.enemyCharacters[1], characterDatabase.enemyCharacters[2] };
            case 4:
                return new[] { characterDatabase.enemyCharacters[1], characterDatabase.enemyCharacters[2], characterDatabase.enemyCharacters[3] };
            case 5:
                return new[] { characterDatabase.enemyCharacters[2], characterDatabase.enemyCharacters[3], characterDatabase.enemyCharacters[4] };
            case 6:
                return new[] { characterDatabase.enemyCharacters[3], characterDatabase.enemyCharacters[4], characterDatabase.enemyCharacters[5] };
            case 7:
                return new[] { characterDatabase.enemyCharacters[4], characterDatabase.enemyCharacters[5], characterDatabase.enemyCharacters[6] };
            case 8:
                return new[] { characterDatabase.enemyCharacters[5], characterDatabase.enemyCharacters[6], characterDatabase.enemyCharacters[7], characterDatabase.enemyCharacters[8] };
            case 9:
                return new[] { characterDatabase.enemyCharacters[6], characterDatabase.enemyCharacters[7], characterDatabase.enemyCharacters[8], characterDatabase.enemyCharacters[9] };
            default:
                return new CharacterData[0];
        }
    }
}
