using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionUI : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject difficultyPanel; // Difficulty selection panel

    [Header("Difficulty Buttons")]
    [SerializeField] private Button easyButton;
    [SerializeField] private Button mediumButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button closeButton;

    [Header("Level Buttons")]
    [SerializeField] private LevelButton[] levelButtons; // Array of all level buttons in the UI

    [Header("Level Selection")]
    [SerializeField] private int selectedLevelIndex;

    private void Awake()
    {
        if (Time.timeScale != 1) {
            Time.timeScale = 1;
        }
    }

    private void Start()
    {
        difficultyPanel.SetActive(false);

        // Add listeners for difficulty selection
        easyButton.onClick.AddListener(() => SelectDifficulty(Difficulty.Easy));
        mediumButton.onClick.AddListener(() => SelectDifficulty(Difficulty.Medium));
        hardButton.onClick.AddListener(() => SelectDifficulty(Difficulty.Hard));
        closeButton.onClick.AddListener(() => OnDifficultyPanelClosed());

        // Initialize all level buttons
        UpdateAllLevelButtons();
    }

    public void OnLevelButtonClicked(int levelIndex)
    {
        AudioManager.Instance.PlaySFX("Button Click");
        selectedLevelIndex = levelIndex;
        difficultyPanel.SetActive(true);

        // Enable difficulty buttons based on unlock progress
        easyButton.interactable = true; // Easy is always unlocked
        mediumButton.interactable = SaveManager.Instance.IsDifficultyUnlocked(levelIndex, Difficulty.Medium);
        hardButton.interactable = SaveManager.Instance.IsDifficultyUnlocked(levelIndex, Difficulty.Hard);
    }

    public void OnDifficultyPanelClosed()
    {
        difficultyPanel.SetActive(false);
    }

    private void SelectDifficulty(Difficulty difficulty)
    {
        AudioManager.Instance.PlaySFX("Button Click");
        difficultyPanel.SetActive(false);

        // Set the selected difficulty in the GameManager
        GameManager.Instance.SetDifficulty(difficulty);

        // Load the selected level
        LevelManager.Instance.LoadLevel(selectedLevelIndex);
    }

    public void UpdateAllLevelButtons()
    {
        foreach (var levelButton in levelButtons) {
            levelButton.UpdateButtonState();
        }
    }
}
