using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarracksUpgradeController : MonoBehaviour
{
    [Header("UI Panel Elements")]
    [SerializeField] private GameObject upgradeBarracksPanel;

    [Header("Text Elements")]
    [SerializeField] private TextMeshProUGUI maxLevelText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI currentHpText;
    [SerializeField] private TextMeshProUGUI nextLevelHpText;

    [Header("Barracks Data")]
    [SerializeField] private BarracksData barracksData; // Reference to the ScriptableObject

    [Header("Buttons")]
    [SerializeField] private Button barracksLevelUpButton;
    [SerializeField] private Button openBarracksPanelButton;
    [SerializeField] private Button closeBarracksPanelButton;

    [Header("Images")]
    [SerializeField] private Image arrowImage;
    [SerializeField] private Image barracksImage;
    [SerializeField] private Image currentBarracksLevelImage;
    [SerializeField] private Image nextLevelBarracksImage;

    [Header("Barracks Level")]
    [SerializeField] private int level;

    private void Awake()
    {
        upgradeBarracksPanel.SetActive(false);

        openBarracksPanelButton.onClick.AddListener(() => { OnOpenBarracksPanel(); });
        closeBarracksPanelButton.onClick.AddListener(() => { OnCloseBarracksPanel(); });
    }

    private void Start()
    {
        //SaveManager.Instance.gameData.coinTotal = 100000; // REMOVE

        InitUnlockButton();
        level = SaveManager.Instance.gameData.barracksUpgradeIndex;

        barracksImage.sprite = barracksData.barracks[level].upgradeSprites[0];
        if (level >= 4) { level = 4; MaxLevelReached(); return; }

        costText.text = $"{GetLevelUpCost(level).ToString()} Coins";
        currentBarracksLevelImage.sprite = barracksData.barracks[level].upgradeSprites[0];
        nextLevelBarracksImage.sprite = barracksData.barracks[level + 1].upgradeSprites[0];
        currentHpText.text = barracksData.barracks[level].health.ToString() + " Hp";
        nextLevelHpText.text = barracksData.barracks[level + 1].health.ToString() + " Hp";
        if (level < 4) {
            maxLevelText.enabled = false;
        }
    }

    public void InitUnlockButton()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.gameData != null) {
            int index = SaveManager.Instance.gameData.barracksUpgradeIndex;

            // Update button state based on unlock status
            if (index > 4) {
                SetButtonState(false); // Disable the button if already unlocked
            } else {
                barracksLevelUpButton.onClick.RemoveAllListeners(); // Clear existing listeners
                barracksLevelUpButton.onClick.AddListener(() => LevelUpBarracks()); // Assign unlock function
            }
        }
    }

    private void LevelUpBarracks()
    {
        AudioManager.Instance.PlaySFX("Button Click");
        int cost = GetLevelUpCost(level);
        if (SaveManager.Instance.gameData.coinTotal >= cost) {
            SaveManager.Instance.gameData.coinTotal -= cost;
            level++;
            SaveManager.Instance.gameData.barracksUpgradeIndex = level;
            SaveManager.Instance.SaveGame();

            if (level >= 4) { level = 4; MaxLevelReached(); return; }
            barracksImage.sprite = barracksData.barracks[level].upgradeSprites[0];
            currentBarracksLevelImage.sprite = barracksData.barracks[level].upgradeSprites[0];
            if (level < 4) {
                nextLevelBarracksImage.sprite = barracksData.barracks[level + 1].upgradeSprites[0];
                nextLevelHpText.text = barracksData.barracks[level + 1].health.ToString() + " Hp";
            }
            currentHpText.text = barracksData.barracks[level].health.ToString() + " Hp";

            costText.text = $"{GetLevelUpCost(level)} Coins";
        } else {
            Debug.Log($"Not enough coins to level up, need {GetLevelUpCost(level)}");
        }
    }

    private int GetLevelUpCost(int level)
    {
        return Mathf.RoundToInt(100 * Mathf.Pow(1.8f, level + 1));
    }

    private void MaxLevelReached()
    {
        barracksLevelUpButton.interactable = false;
        openBarracksPanelButton.interactable = false;
        openBarracksPanelButton.GetComponentInChildren<TextMeshProUGUI>().text = "Max Level";
        currentHpText.text = barracksData.barracks[level].health.ToString() + " Hp";
        nextLevelHpText.enabled = false;
        nextLevelBarracksImage.enabled = false;
        costText.enabled = false;
        arrowImage.enabled = false;
        maxLevelText.enabled = true;
    }

    private void SetButtonState(bool interactable)
    {
        barracksLevelUpButton.interactable = interactable;
    }

    private void OnOpenBarracksPanel()
    {
        AudioManager.Instance.PlaySFX("Button Click");
        upgradeBarracksPanel.SetActive(true);
    }

    private void OnCloseBarracksPanel()
    {
        AudioManager.Instance.PlaySFX("Button Click");
        upgradeBarracksPanel.SetActive(false);
    }
}
