using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Transform coinPrefabSpawnPos;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button levelCompleteLevelSelectButton;
    [SerializeField] private Button gameOverLevelSelectButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private List<TextMeshProUGUI> buttonTextToDisable = new List<TextMeshProUGUI>();
    [SerializeField] private List<Button> buttonsToNonInteract = new List<Button>();
    [SerializeField] private List<Image> buttonIconsToDisable = new List<Image>();

    private void Awake()
    {
        levelCompleteLevelSelectButton.onClick.AddListener(() => LoadLevelSelectScene());
        gameOverLevelSelectButton.onClick.AddListener(() => LoadLevelSelectScene());
        resumeButton.onClick.AddListener(() => ClosePausePanel());
        quitButton.onClick.AddListener(() => LoadLevelSelectScene());
    }

    private void Start()
    {
        AudioManager.Instance.PlayGameMusic();

        gameOverPanel.SetActive(false);
        levelCompletePanel.SetActive(false);
        pausePanel.SetActive(false);

        foreach (TextMeshProUGUI text in buttonTextToDisable) {
            text.enabled = false;
        }

        foreach (Button button in buttonsToNonInteract) {
            button.interactable = false;
        }

        foreach (Image icon in buttonIconsToDisable) {
            icon.enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!pausePanel.activeInHierarchy && !FindAnyObjectByType<OptionsManager>().optionsPanel.activeInHierarchy) {
                OpenPausePanel();
            } else if (pausePanel.activeInHierarchy && !FindAnyObjectByType<OptionsManager>().optionsPanel.activeInHierarchy) {
                ClosePausePanel();
            }
        }
    }

    public bool IsPauePanelOpen()
    {
        return pausePanel.activeInHierarchy;
    }

    public void OpenPausePanel()
    {
        AudioManager.Instance.PlaySFX("Button Click");
        //Debug.Log("Pause panel opened");
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ClosePausePanel()
    {
        AudioManager.Instance.PlaySFX("Button Click");
        //Debug.Log("Pause panel closed");
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OpenLevelCompletePanel(int level, Difficulty difficulty)
    {
        levelCompletePanel.SetActive(true);
        SetDescriptionText(level, difficulty);
        Time.timeScale = 0f;
    }

    public void SetDescriptionText(int level, Difficulty difficulty)
    {
        descriptionText.text = $"Completed level {level + 1} on {difficulty}.";
    }

    private void LoadLevelSelectScene()
    {
        AudioManager.Instance.PlayMenuMusic();
        SceneManager.LoadScene("LevelSelect");
    }

    public void SpawnCoinPrefabUI(int amount)
    {
        Canvas canvas = FindAnyObjectByType(typeof(Canvas)) as Canvas;
        GameObject coinPrefabGO = Instantiate(coinPrefab, coinPrefabSpawnPos.position, Quaternion.identity, canvas.transform);
        coinPrefabGO.GetComponent<AddRemoveCoinPrefab>().coinAmount = amount;
    }
}
