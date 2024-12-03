using UnityEngine;
using UnityEngine.UI;

public class UnlockButton : MonoBehaviour
{
    public string characterName; // Name of the character this button unlocks
    public Button unlockButton;  // The button component

    private void Start()
    {
        InitUnlockButton();
    }

    public void InitUnlockButton()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.gameData != null) {
            bool isUnlocked = SaveManager.Instance.gameData.characters.Exists(c => c.characterName == characterName && c.isUnlocked);

            // Update button state based on unlock status
            if (isUnlocked) {
                SetButtonState(false); // Disable the button if already unlocked
            } else {
                unlockButton.onClick.RemoveAllListeners(); // Clear existing listeners
                unlockButton.onClick.AddListener(() => UnlockCharacter()); // Assign unlock function
            }
        }
    }

    private void UnlockCharacter()
    {
        if (SaveManager.Instance != null) {
            CharacterManager characterManager = FindAnyObjectByType<CharacterManager>();
            if (characterManager != null) {
                characterManager.UnlockCharacter(characterName);
                SetButtonState(false); // Disable the button after unlocking
            }
        }
    }

    private void SetButtonState(bool interactable)
    {
        unlockButton.interactable = interactable;
    }
}
