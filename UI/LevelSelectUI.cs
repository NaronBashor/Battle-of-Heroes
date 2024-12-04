using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionUI : MonoBehaviour
{
    public GameObject difficultyPanel; // Difficulty selection panel
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;

    private int selectedLevelIndex;

    private void Start()
    {
        difficultyPanel.SetActive(false);

        easyButton.onClick.AddListener(() => SelectDifficulty(Difficulty.Easy));
        mediumButton.onClick.AddListener(() => SelectDifficulty(Difficulty.Medium));
        hardButton.onClick.AddListener(() => SelectDifficulty(Difficulty.Hard));
    }

    public void OnLevelButtonClicked(int levelIndex)
    {
        selectedLevelIndex = levelIndex;
        difficultyPanel.SetActive(true);

        // Enable buttons based on unlock progress
        easyButton.interactable = true; // Easy is always unlocked
        mediumButton.interactable = SaveManager.Instance.IsDifficultyUnlocked(levelIndex, Difficulty.Medium);
        hardButton.interactable = SaveManager.Instance.IsDifficultyUnlocked(levelIndex, Difficulty.Hard);
    }

    private void SelectDifficulty(Difficulty difficulty)
    {
        difficultyPanel.SetActive(false);
        GameManager.Instance.SetDifficulty(difficulty);
        LevelManager.Instance.LoadLevel(selectedLevelIndex);
    }
}
