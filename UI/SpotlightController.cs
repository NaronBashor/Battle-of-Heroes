using UnityEngine;

public class SpotlightController : MonoBehaviour
{
    [SerializeField] private GameObject spotlightPanel;

    private void Start()
    {
        if (spotlightPanel == null) { return; }
        spotlightPanel.SetActive(false);
    }

    private void Update()
    {
        if (spotlightPanel == null) { return; }
        if (Input.anyKey) {
            if (spotlightPanel.activeInHierarchy) {
                spotlightPanel.SetActive(false);
            }
        }
    }

    public void OpenSpotlightPanel()
    {
        if (spotlightPanel == null) { return; }
        spotlightPanel.SetActive(true);
    }

    public void CloseSpotlightPanel()
    {
        if (spotlightPanel == null) { return; }
        spotlightPanel.SetActive(false);
    }
}
