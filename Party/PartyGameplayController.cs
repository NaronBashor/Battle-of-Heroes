using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PartyGameplayController : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private List<Button> gameplayButtons; // Assign 6 buttons for the party

    [Header("UI Sprites")]
    [SerializeField] private Sprite emptySlotSprite; // Sprite for empty slots

    [Header("Character Settings")]
    [SerializeField] private int maxActiveCharacters = 5; // Maximum active characters allowed
    [SerializeField] private int currentActiveCharacters = 0; // Track active characters

    [Header("Cooldown Timers")]
    [SerializeField] private Dictionary<string, float> cooldownTimers = new Dictionary<string, float>(); // Cooldown timers for characters

    private void Start()
    {
        LoadParty(); // Initialize party buttons
    }

    private void LoadParty()
    {
        var party = SaveManager.Instance.gameData.selectedParty;

        for (int i = 0; i < gameplayButtons.Count; i++) {
            Button button = gameplayButtons[i];
            Image buttonImage = button.GetComponent<Image>();

            if (i < party.Count) {
                // Set the button sprite and enable it
                string characterName = party[i];
                CharacterData characterData = FindAnyObjectByType<Spawner>().playerDatabase.GetCharacterByName(characterName);

                if (characterData != null) {
                    buttonImage.sprite = characterData.characterButtonSprite;
                    button.interactable = true;

                    // Assign button click event to spawn character
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => TrySpawnCharacter(characterName));
                }
            } else {
                // Empty slot for unused party members
                buttonImage.sprite = emptySlotSprite;
                button.interactable = false;
                button.onClick.RemoveAllListeners();
            }
        }
    }

    public void TrySpawnCharacter(string characterName)
    {
        CharacterData characterData = FindAnyObjectByType<Spawner>().playerDatabase.GetCharacterByName(characterName);

        if (characterData == null) {
            Debug.LogError($"Character {characterName} not found!");
            return;
        }

        // Check cooldown
        if (cooldownTimers.ContainsKey(characterName) && Time.time < cooldownTimers[characterName]) {
            float remainingTime = cooldownTimers[characterName] - Time.time;
            Debug.Log($"{characterName} is on cooldown! Time left: {remainingTime:F1} seconds.");
            return;
        }

        // Check if the player has enough coins
        if (SaveManager.Instance.gameData.coinTotal < characterData.spawnCost) {
            Debug.Log("Not enough coins to spawn this character!");
            return;
        }

        // Check if the maximum active characters limit is reached
        if (currentActiveCharacters >= maxActiveCharacters) {
            Debug.Log("Maximum active characters reached!");
            return;
        }

        // Deduct coins and spawn the character
        SaveManager.Instance.gameData.coinTotal -= characterData.spawnCost;
        SaveManager.Instance.SaveGame(); // Save updated coin count

        FindAnyObjectByType<Spawner>().TrySpawnPlayerCharacter(characterName);
        currentActiveCharacters++;

        // Apply cooldown
        cooldownTimers[characterName] = Time.time + characterData.spawnCooldown;

        Debug.Log($"{characterName} spawned! Remaining coins: {SaveManager.Instance.gameData.coinTotal}");
    }

    public void OnCharacterDeath()
    {
        // Decrease active character count
        currentActiveCharacters = Mathf.Max(0, currentActiveCharacters - 1);
    }

    private void Update()
    {
        // Update button states based on cooldowns
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        var party = SaveManager.Instance.gameData.selectedParty;

        for (int i = 0; i < gameplayButtons.Count; i++) {
            Button button = gameplayButtons[i];

            if (i < party.Count) {
                string characterName = party[i];
                CharacterData characterData = FindAnyObjectByType<Spawner>().playerDatabase.GetCharacterByName(characterName);

                if (characterData != null) {
                    // Enable button only if off cooldown and coins are sufficient
                    bool isOffCooldown = !cooldownTimers.ContainsKey(characterName) || Time.time >= cooldownTimers[characterName];
                    bool hasEnoughCoins = SaveManager.Instance.gameData.coinTotal >= characterData.spawnCost;

                    button.interactable = isOffCooldown && hasEnoughCoins && currentActiveCharacters < maxActiveCharacters;
                }
            }
        }
    }
}
