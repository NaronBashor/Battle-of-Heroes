using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        if (!SaveManager.Instance.SaveExists()) {
            continueButton.interactable = false;
        } else {
            continueButton.interactable = true;
        }

        newGameButton.onClick.AddListener(() =>
        {
            CreateNewGame();
        });
        continueButton.onClick.AddListener(() =>
        {
            ContinueSaveGame();
        });
        exitButton.onClick.AddListener(() =>
        {
            OnExitButtonPressed();
        });
    }

    private void CreateNewGame()
    {
        AudioManager.Instance.PlaySFX("Button Click");
        SaveManager.Instance.DeleteSave();
        SceneManager.LoadScene("Heroes");

        GameManager.Instance.SetGameState(GameManager.GameState.Heroes);
    }

    private void ContinueSaveGame()
    {
        AudioManager.Instance.PlaySFX("Button Click");
        SaveManager.Instance.LoadGame();
        SceneManager.LoadScene("Heroes");

        GameManager.Instance.SetGameState(GameManager.GameState.Heroes);
    }

    private void OnExitButtonPressed()
    {
        AudioManager.Instance.PlaySFX("Button Click");
        Application.Quit();
    }
}
