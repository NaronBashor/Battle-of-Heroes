using System.Linq;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public CharacterDatabase characterDatabase;

    public void UnlockCharacter(string characterName)
    {
        var character = SaveManager.Instance.gameData.characters.FirstOrDefault(c => c.characterName == characterName);
        if (character != null) {
            character.isUnlocked = true;
            SaveManager.Instance.SaveGame();
            //Debug.Log($"{characterName} unlocked!");
        }
    }
}
