using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public Material material;
    public Vector2[] spotlightPostiions;
    public float[] spotlightRadius;
    public GameObject[] tutorialWindows; // Assign windows in Inspector
    public GameObject tutorialPanel; // Assign windows in Inspector
    public GameObject spotlight; // Assign windows in Inspector
    private int currentWindowIndex = 0;

    public Button[] buttons;

    private void Awake()
    {
        foreach (Button button in buttons) {
            button.onClick.AddListener(() => NextWindow());
        }
    }

    void Start()
    {
        material.SetFloat("_HighlightCenterX", spotlightPostiions[currentWindowIndex].x); // Set X center
        material.SetFloat("_HighlightCenterY", spotlightPostiions[currentWindowIndex].y); // Set Y center
        material.SetFloat("_HighlightRadius", spotlightRadius[currentWindowIndex]);  // Set radius

        if (SaveManager.Instance.gameData.isTutorialCompleted) { tutorialPanel.SetActive(false); spotlight.SetActive(false); return; }
        ShowWindow(currentWindowIndex);
    }

    public void NextWindow()
    {
        if (currentWindowIndex < tutorialWindows.Length - 1) {
            tutorialWindows[currentWindowIndex].SetActive(false);
            currentWindowIndex++;
            material.SetFloat("_HighlightCenterX", spotlightPostiions[currentWindowIndex].x); // Set X center
            material.SetFloat("_HighlightCenterY", spotlightPostiions[currentWindowIndex].y); // Set Y center
            material.SetFloat("_HighlightRadius", spotlightRadius[currentWindowIndex]);  // Set radius
            tutorialWindows[currentWindowIndex].SetActive(true);
            tutorialWindows[currentWindowIndex].GetComponentInChildren<TextMeshProUGUI>().text = $"{currentWindowIndex + 1}/{tutorialWindows.Length}";
        } else {
            // Go to the next scene
            if (SceneManager.GetActiveScene().name == "Level") { SaveManager.Instance.CompleteTutorial(); spotlight.SetActive(false); SaveManager.Instance.gameData.coinTotal += 15; }
            spotlight.SetActive(false);
            tutorialWindows[currentWindowIndex].SetActive(false);
            SaveManager.Instance.SaveGame();
        }
    }

    public void ShowWindow(int index)
    {
        for (int i = 0; i < tutorialWindows.Length; i++) {
            tutorialWindows[i].SetActive(i == index);
        }
    }
}
