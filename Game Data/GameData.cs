using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<CharacterSaveData> characters = new List<CharacterSaveData>();
    public int coinTotal = 0;
    public int playerLevel = 1;
    public bool isPlayerCharacter;
    public int barracksUpgradeIndex = 0;

    // Add a list to store the party
    public List<string> selectedParty = new List<string>();

    // New options fields
    public bool isMusicEnabled = true; // Default: music is on
    public float musicVolume = 1.0f;   // Default: max volume

    // New: Level and difficulty progress
    public Dictionary<int, LevelProgress> levelProgress = new Dictionary<int, LevelProgress>();

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

        // Initialize level progress
        levelProgress.Clear();
        for (int i = 0; i < 10; i++) // Assume 10 levels
        {
            levelProgress[i] = new LevelProgress();
        }
    }
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
}
