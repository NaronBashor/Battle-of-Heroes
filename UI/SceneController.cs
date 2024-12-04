using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private string sceneName;

    public void ChangeScene(string sceneName)
    {
        switch (sceneName) {
            case "Shop":
                GameManager.Instance.SetGameState(GameManager.GameState.Shop);
                break;
            case "Heroes":
                GameManager.Instance.SetGameState(GameManager.GameState.Heroes);
                break;
            case "Arena":
                GameManager.Instance.SetGameState(GameManager.GameState.Arena);
                break;
            //case "Game":
            //    GameManager.Instance.SetGameState(GameManager.GameState.Game);
            //    break;
            case "LevelSelect":
                if (SaveManager.Instance.gameData.selectedParty.Count < 1) {
                    FindAnyObjectByType<SpotlightController>().OpenSpotlightPanel();
                    return;
                }
                GameManager.Instance.SetGameState(GameManager.GameState.Game);
                break;
        }

        SceneManager.LoadScene(sceneName);
    }
}
