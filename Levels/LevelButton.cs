using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header("Level Information")]
    [SerializeField] private int levelIndex;

    [Header("UI Components")]
    [SerializeField] private Image buttonImage;
    [SerializeField] private Button button;

    [Header("Sprites")]
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite availableSprite;
    [SerializeField] private Sprite oneStarSprite;
    [SerializeField] private Sprite twoStarSprite;
    [SerializeField] private Sprite threeStarSprite;

    private void Start()
    {
        // Assign click listener for this button
        button.onClick.AddListener(() => FindAnyObjectByType<LevelSelectionUI>().OnLevelButtonClicked(levelIndex));
        UpdateButtonState();
    }

    public void UpdateButtonState()
    {
        // Get level progress from SaveManager
        LevelProgressEntry progress = SaveManager.Instance.gameData.levelProgress[levelIndex];

        // Set button sprite and interactivity based on progress
        if (progress == null || progress.progress.starsEarned == 0) // Level is locked
        {
            buttonImage.sprite = lockedSprite;
            button.interactable = false;
        } else if (progress.progress.starsEarned == 1) // One star earned
          {
            buttonImage.sprite = oneStarSprite;
            button.interactable = true;
        } else if (progress.progress.starsEarned == 2) // Two stars earned
          {
            buttonImage.sprite = twoStarSprite;
            button.interactable = true;
        } else if (progress.progress.starsEarned == 3) // Three stars earned
          {
            buttonImage.sprite = threeStarSprite;
            button.interactable = true;
        } else // Level is unlocked but not completed
          {
            buttonImage.sprite = availableSprite;
            button.interactable = true;
        }
    }
}
