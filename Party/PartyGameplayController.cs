using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PartyGameplayController : MonoBehaviour
{
    public List<Button> gameplayButtons; // Assign 6 buttons for the gameplay party
    public Sprite emptySlotSprite;      // Sprite for empty slots

    private void Start()
    {
        LoadParty();
    }

    private void LoadParty()
    {
        var party = SaveManager.Instance.gameData.selectedParty;

        for (int i = 0; i < gameplayButtons.Count; i++) {
            Button button = gameplayButtons[i];
            Image buttonImage = button.GetComponent<Image>();

            if (i < party.Count) {
                // Get the character data and set the button's sprite
                string characterName = party[i];
                var characterData = FindAnyObjectByType<Spawner>().playerDatabase.GetCharacterByName(characterName);

                if (characterData != null) {
                    buttonImage.sprite = characterData.characterButtonSprite;
                    button.interactable = true;

                    // Add logic for button clicks (e.g., spawn character)
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => OnCharacterButtonClicked(characterName));
                }
            } else {
                // Set the button to empty if no character is assigned
                buttonImage.sprite = emptySlotSprite;
                button.interactable = false;
                button.onClick.RemoveAllListeners();
            }
        }
    }

    private void OnCharacterButtonClicked(string characterName)
    {
        FindAnyObjectByType<Spawner>().TrySpawnPlayerCharacter(characterName);
        Debug.Log($"Spawn character: {characterName}");
    }
}
