using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PartyScreenController : MonoBehaviour
{
    [System.Serializable]
    public class PartyButton
    {
        public Button button;             // Reference to the button
        public Button levelUpButton;
        public string characterName;      // Name of the character associated with this button
        public Sprite characterImage;      // Reference to the button's image (sprite)
    }

    [Header("Player Data")]
    [SerializeField] private CharacterDatabase playerDatabase;

    [Header("UI Components")]
    [SerializeField] private Image playerSprite;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI characterLevelText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI attackTypeText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI moveSpeedText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private TextMeshProUGUI upgradeHealthValueText;
    [SerializeField] private TextMeshProUGUI upgradeDamageValueText;
    [SerializeField] private TextMeshProUGUI upgradeAttackSpeedValueText;

    [Header("Character Details")]
    [SerializeField] private string characterName;

    [Header("Level Up Panel")]
    [SerializeField] private GameObject levelUpPanel;

    [Header("Party Management")]
    [SerializeField] private List<PartyButton> partyButtons; // List of inventory buttons (right side)
    [SerializeField] private List<Button> partySlots; // References to the party slot buttons (left side)
    [SerializeField] private Sprite emptySlotSprite; // Sprite for an empty slot
    [SerializeField] private int maxPartySize = 6; // Maximum number of characters in the party

    [Header("Selected Party")]
    [SerializeField] private List<(string characterName, Sprite characterSprite)> selectedParty = new List<(string, Sprite)>(); // Tracks selected party members

    private void Start()
    {
        levelUpPanel.SetActive(false);
        UpdatePartyScreen();
    }

    public void UpdatePartyScreen()
    {
        // Update inventory buttons
        foreach (var partyButton in partyButtons) {
            // Find the character in the SaveManager
            var characterSaveData = SaveManager.Instance.gameData.characters.Find(c =>
                c.characterName.Trim().Equals(partyButton.characterName.Trim(), System.StringComparison.OrdinalIgnoreCase));

            // If the character exists, update button interactability
            if (characterSaveData != null) {
                // Disable the button if the character is already in the party
                bool isInteractable = characterSaveData.isUnlocked && !selectedParty.Exists(p => p.characterName == characterSaveData.characterName);
                partyButton.button.interactable = isInteractable;
                partyButton.levelUpButton.interactable = isInteractable;

                // Assign the OnClick listener to select the character
                partyButton.button.onClick.RemoveAllListeners();
                partyButton.button.onClick.AddListener(() => OnCharacterSelected(partyButton.characterName, partyButton.characterImage));
            } else {
                partyButton.button.interactable = false;
                partyButton.levelUpButton.interactable = false;
                Debug.LogWarning($"Character {partyButton.characterName} not found in SaveManager!");
            }

            // Force refresh button state
            ForceRefreshButton(partyButton.button);
        }

        // Update the left-side party slots
        UpdatePartySlots();
    }

    private void ForceRefreshButton(Button button)
    {
        button.enabled = false;
        button.enabled = true;
    }

    public void OnCharacterSelected(string characterName, Sprite characterSprite)
    {
        AudioManager.Instance.PlaySFX("Button Click");
        if (selectedParty.Count < maxPartySize) {
            // Add character to the party
            selectedParty.Add((characterName, characterSprite));
            //Debug.Log($"Added {characterName} to the party.");
        } else {
            Debug.Log("Party is full! Cannot add more characters.");
        }

        // Update both inventory and party slots
        UpdatePartyScreen();
    }

    public void OnPartySlotClicked(int slotIndex)
    {
        AudioManager.Instance.PlaySFX("Button Click");
        if (slotIndex < selectedParty.Count) {
            // Remove the character from the party
            var removedCharacter = selectedParty[slotIndex];
            selectedParty.RemoveAt(slotIndex);
            //Debug.Log($"Removed {removedCharacter.characterName} from the party.");

            // Update both inventory and party slots
            UpdatePartyScreen();
        }
    }

    private void UpdatePartySlots()
    {
        // Update the left-side "Party Circles" based on the selected party
        for (int i = 0; i < partySlots.Count; i++) {
            Button slotButton = partySlots[i];
            Image slotImage = slotButton.GetComponent<Image>();

            if (i < selectedParty.Count) {
                // Use the sprite from the selected party
                slotImage.sprite = selectedParty[i].characterSprite;
                slotImage.enabled = true;

                // Assign OnClick to remove the character from the party
                slotButton.onClick.RemoveAllListeners();
                int index = i; // Capture index for the lambda
                slotButton.onClick.AddListener(() => OnPartySlotClicked(index));
            } else {
                // Set empty slot sprite for unused slots
                slotImage.sprite = emptySlotSprite;
                slotImage.enabled = true;

                // Remove any existing listeners on empty slots
                slotButton.onClick.RemoveAllListeners();
            }
        }
        SaveParty();
    }

    public void OnLevelUpClicked()
    {
        AudioManager.Instance.PlaySFX("Button Click");
        // Get the character's saved data
        var characterSaveData = SaveManager.Instance.gameData.characters.Find(c => c.characterName == characterName);

        //SaveManager.Instance.gameData.coinTotal = 100000; // REMOVE

        if (characterSaveData != null) {
            // Check if the player has enough coins to level up
            int cost = GetLevelUpCost(characterSaveData.level);
            if (SaveManager.Instance.gameData.coinTotal >= cost) {
                SaveManager.Instance.gameData.coinTotal -= cost; // Deduct the cost
                var characterData = playerDatabase.GetCharacterByName(characterSaveData.characterName);
                if (characterData != null) {
                    // Apply level-up changes
                    characterSaveData.LevelUp(characterData);

                    SaveManager.Instance.SaveGame(); // Save the updated data
                    UpdatePartyScreen(); // Refresh UI
                    //Debug.Log($"{characterName} leveled up to level {characterSaveData.level}!");
                }
            } else {
                Debug.Log($"Need {cost} coins to purchase upgrade!");
            }
        } else {
            Debug.LogError($"Character {characterName} not found in GameData!");
        }

        RefreshData(characterName); // Refresh the displayed data
    }


    public void OpenLevelUpPanel(string characterName)
    {
        AudioManager.Instance.PlaySFX("Button Click");
        this.characterName = characterName;
        levelUpPanel.SetActive(true);

        RefreshData(characterName);
    }

    private void RefreshData(string characterName)
    {
        // Find the character's saved data
        var characterSaveData = SaveManager.Instance.gameData.characters.Find(c => c.characterName == characterName);
        if (characterSaveData == null) {
            Debug.LogError($"Character {characterName} not found in GameData!");
            return;
        }

        // Find the default character data
        var characterData = playerDatabase.GetCharacterByName(characterSaveData.characterName);
        if (characterData == null) {
            Debug.LogError($"Character {characterName} not found in CharacterDatabase!");
            return;
        }

        // Update UI
        characterNameText.text = characterSaveData.characterName;
        characterLevelText.text = $"Level: {characterSaveData.level.ToString("F0")}";
        playerSprite.sprite = characterData.characterSprite;
        healthText.text = characterSaveData.health.ToString("F0"); // Use updated damage
        attackTypeText.text = characterData.attackType.ToString();
        damageText.text = characterSaveData.damage.ToString("F0"); // Use updated damage
        attackSpeedText.text = $"{characterSaveData.attackCooldown.ToString("F2")}/sec"; // Use updated attack speed
        moveSpeedText.text = characterData.speed.ToString("F0");

        upgradeCostText.text = GetLevelUpCost(characterSaveData.level).ToString("F0");
        upgradeHealthValueText.text = $"+{characterData.healthIncreasePerLevel.ToString("F0")}";
        upgradeDamageValueText.text = $"+{characterData.damageIncreasePerLevel.ToString("F0")}";
        upgradeAttackSpeedValueText.text = $"-{characterData.attackSpeedIncreasePerLevel.ToString("F2")}";
    }

    public void CloseLevelUpPanel()
    {
        AudioManager.Instance.PlaySFX("Button Click");
        levelUpPanel.SetActive(false);
    }

    public int GetLevelUpCost(int currentLevel)
    {
        return 100 * currentLevel; // Example: 100 coins per level
    }

    public void SaveParty()
    {
        SaveManager.Instance.gameData.selectedParty.Clear();

        foreach (var partyMember in selectedParty) {
            SaveManager.Instance.gameData.selectedParty.Add(partyMember.characterName);
        }

        SaveManager.Instance.SaveGame(); // Save the updated game data
        //Debug.Log("Party saved!");
    }

    public void OnDisable()
    {
        SaveParty();
    }
}
