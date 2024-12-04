using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Slider volumeSlider;

    [Header("Options Panel")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Button openOptionsPanelButton;
    [SerializeField] private Button closeOptionsPanelButton;

    [Header("Menu Buttons")]
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button moreGamesButton;
    [SerializeField] private Button exitButton;

    [Header("Sprites")]
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;

    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI currentCoinCountText;

    private void Awake()
    {
        ApplySettings();

        mainMenuButton.onClick.AddListener(() =>
        {
            LoadMainMenu();
        });
        openOptionsPanelButton.onClick.AddListener(() =>
        {
            OpenOptionsPanel();
        });
        closeOptionsPanelButton.onClick.AddListener(() =>
        {
            CloseOptionsPanel();
        });
        creditsButton.onClick.AddListener(() =>
        {
            OnCreditsButtonPressed();
        });
        moreGamesButton.onClick.AddListener(() =>
        {
            OnMoreGamesButtonPressed();
        });
        exitButton.onClick.AddListener(() =>
        {
            ExitGame();
        });

        optionsPanel.SetActive(false);
    }

    private void Start()
    {
        // Initialize the UI with saved settings
        if (SaveManager.Instance != null && SaveManager.Instance.gameData != null) {
            musicToggle.isOn = SaveManager.Instance.gameData.isMusicEnabled;
            volumeSlider.value = SaveManager.Instance.gameData.musicVolume;
        }

        // Add listeners for changes
        musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
    }

    private void Update()
    {
        currentCoinCountText.text = SaveManager.Instance.gameData.coinTotal.ToString();
    }

    public bool IsOptionsPanelOpen()
    {
        return optionsPanel.activeInHierarchy;
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenOptionsPanel()
    {
        Time.timeScale = 0f;
        openOptionsPanelButton.image.sprite = inactiveSprite;
        optionsPanel.SetActive(true);
    }

    public void CloseOptionsPanel()
    {
        Time.timeScale = 1f;
        openOptionsPanelButton.image.sprite = activeSprite;
        optionsPanel.SetActive(false);
    }

    private void OnMusicToggleChanged(bool isEnabled)
    {
        if (SaveManager.Instance != null && SaveManager.Instance.gameData != null) {
            SaveManager.Instance.gameData.isMusicEnabled = isEnabled;
            SaveManager.Instance.SaveGame(); // Save changes
        }

        // Enable or disable music in the game
        AudioListener.pause = !isEnabled;
    }

    private void OnVolumeSliderChanged(float volume)
    {
        if (SaveManager.Instance != null && SaveManager.Instance.gameData != null) {
            SaveManager.Instance.gameData.musicVolume = volume;
            SaveManager.Instance.SaveGame(); // Save changes
        }

        // Update audio volume
        AudioListener.volume = volume;
    }

    private void OnCreditsButtonPressed()
    {
        Debug.Log("Credits button pressed.");
    }

    private void OnMoreGamesButtonPressed()
    {
        Application.OpenURL("https://splitrockgames.com");
    }

    private void ApplySettings()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.gameData != null) {
            AudioListener.pause = !SaveManager.Instance.gameData.isMusicEnabled;
            AudioListener.volume = SaveManager.Instance.gameData.musicVolume;
        }
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
