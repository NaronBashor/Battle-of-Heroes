using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Game/Character Database")]
public class CharacterDatabase : ScriptableObject
{
    public List<CharacterData> playerCharacters;
    public List<CharacterData> enemyCharacters;

    public CharacterData GetCharacterByName(string name)
    {
        foreach (var character in playerCharacters) {
            if (character.characterName == name) return character;
        }
        foreach (var character in enemyCharacters) {
            if (character.characterName == name) return character;
        }
        Debug.LogError($"Character {name} not found in the database!");
        return null;
    }
}
