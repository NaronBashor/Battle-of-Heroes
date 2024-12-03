using UnityEngine;

public class BarracksController : MonoBehaviour
{
    public BarracksData barracksData; // Reference to the ScriptableObject
    public GameObject playerBarracks; // Renderer for displaying sprites
    public int barrackIndex = 0; // Index of the current barracks in the list

    private int currentUpgradeLevel = 0; // Current upgrade level
    private int currentDamageState = 0; // Current damage state

    private void Start()
    {
        if (barracksData == null || playerBarracks.GetComponent<SpriteRenderer>() == null) {
            Debug.LogError("BarracksData or SpriteRenderer is not assigned!");
            return;
        }

        barrackIndex = SaveManager.Instance.gameData.barracksUpgradeIndex;
        UpdateSprite(); // Display the initial sprite
    }

    public void UpgradeBarrack()
    {
        if (currentUpgradeLevel < barracksData.barracks[barrackIndex].upgradeSprites.Length - 1) {
            currentUpgradeLevel++;
            currentDamageState = 0; // Reset damage state on upgrade
            UpdateSprite();
        } else {
            Debug.Log("Barracks is already at max upgrade level.");
        }
    }

    public void ApplyDamage()
    {
        if (currentDamageState < barracksData.barracks[barrackIndex].damageSprites.Length - 1) {
            currentDamageState++;
            UpdateSprite();
        } else {
            Debug.Log("Barracks Destroyed, Game Over.");
        }
    }

    private void UpdateSprite()
    {
        Sprite newSprite = currentDamageState == 0
            ? barracksData.GetUpgradeSprite(barrackIndex, currentUpgradeLevel)
            : barracksData.GetDamageSprite(barrackIndex, currentDamageState - 1);

        if (barrackIndex == 3) {
            playerBarracks.GetComponent<Transform>().position += new Vector3(0, 0.2f);
        }
        if (barrackIndex == 4) {
            playerBarracks.GetComponent<Transform>().position += new Vector3(0, 0.4f);
        }

        if (newSprite != null) {
            playerBarracks.GetComponent<SpriteRenderer>().sprite = newSprite;
        } else {
            Debug.LogError("Sprite not found for the current state.");
        }
    }
}
