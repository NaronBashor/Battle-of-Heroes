using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public int levelIndex;
    public Button button;

    private void Start()
    {
        button.onClick.AddListener(() => FindAnyObjectByType<LevelSelectionUI>().OnLevelButtonClicked(levelIndex));
    }
}
