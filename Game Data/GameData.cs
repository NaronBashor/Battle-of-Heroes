using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    [Header("Character Data")]
    [SerializeField] public List<CharacterSaveData> characters = new List<CharacterSaveData>();

    [Header("Player Stats")]
    [SerializeField] public int coinTotal = 0;
    [SerializeField] public int playerLevel = 1;

    [Header("Player Settings")]
    [SerializeField] private bool isPlayerCharacter;

    [Header("Barracks Data")]
    [SerializeField] public int barracksUpgradeIndex = 0;

    // Add a list to store the party
    public List<string> selectedParty = new List<string>();

    // New options fields
    public bool isMusicEnabled = true; // Default: music is on
    public float musicVolume = 1.0f;   // Default: max volume

    // New: Level and difficulty progress
    public List<LevelProgressEntry> levelProgress = new List<LevelProgressEntry>(); // Use List for serialization compatibility

    public GameData(CharacterDatabase database)
    {
        ResetGameData(database);
    }

    public void ResetGameData(CharacterDatabase database)
    {
        // Reset global game data
        coinTotal = 0;
        playerLevel = 1;
        barracksUpgradeIndex = 0;
        selectedParty.Clear();

        // Reset characters
        characters.Clear();
        foreach (var character in database.playerCharacters) {
            characters.Add(new CharacterSaveData(character));
        }

        // Reset level progress
        levelProgress.Clear();
        for (int i = 0; i < 10; i++) // Assume 10 levels
        {
            levelProgress.Add(new LevelProgressEntry {
                levelIndex = i,
                progress = new LevelProgress()
            });
        }
    }

    public LevelProgress GetLevelProgress(int levelIndex)
    {
        var entry = levelProgress.Find(x => x.levelIndex == levelIndex);
        return entry?.progress;
    }

    public void SetLevelProgress(int levelIndex, LevelProgress progress)
    {
        var entry = levelProgress.Find(x => x.levelIndex == levelIndex);
        if (entry != null) {
            entry.progress = progress;
        }
    }
}

[System.Serializable]
public class LevelProgressEntry
{
    public int levelIndex; // The index of the level
    public LevelProgress progress; // Progress data for this level
}

[System.Serializable]
public class CharacterSaveData
{
    public string characterName;
    public int level;
    public bool isUnlocked;
    public int damage;
    public float attackCooldown;
    public float health;

    // Constructor to initialize from CharacterData
    public CharacterSaveData(CharacterData characterData)
    {
        characterName = characterData.characterName;
        level = 1; // Default level
        if (characterData.characterName == "Red-haired Paladin") {
            isUnlocked = true;
        } else {
            isUnlocked = false;
        }
        health = characterData.health;
        damage = characterData.damage;
        attackCooldown = characterData.attackCooldown;
    }

    public void LevelUp(CharacterData data)
    {
        level++;
        health += data.healthIncreasePerLevel;
        damage += data.damageIncreasePerLevel;
        attackCooldown -= data.attackSpeedIncreasePerLevel;
    }
}

[System.Serializable]
public class LevelProgress
{
    public bool easyCompleted = false;
    public bool mediumCompleted = false;
    public bool hardCompleted = false;
    public int starsEarned = 0; // 0: Locked, 1: One star, 2: Two stars, 3: Three stars
}