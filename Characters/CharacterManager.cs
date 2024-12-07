using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public CharacterDatabase characterDatabase;

    public void UnlockCharacter(string characterName)
    {
        AudioManager.Instance.PlaySFX("Button Click");
        var character = SaveManager.Instance.gameData.characters.FirstOrDefault(c => c.characterName == characterName);
        if (character != null) {
            character.isUnlocked = true;
            SaveManager.Instance.SaveGame();
            //Debug.Log($"{characterName} unlocked!");
        }
    }
}
