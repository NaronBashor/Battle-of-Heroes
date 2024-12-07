using UnityEngine;
using UnityEngine.UI;

public class TrialPrefab : MonoBehaviour
{
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        purchaseButton.onClick.AddListener(() => DirectPlayerToPurchase());
        exitButton.onClick.AddListener(() => QuitGame());
    }

    private void DirectPlayerToPurchase()
    {
        Application.OpenURL("https://splitrockgames.com");
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
