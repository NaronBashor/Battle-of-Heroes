using UnityEngine;

public class BarracksController : MonoBehaviour
{
    public BarracksData barracksData; // Reference to the ScriptableObject
    public GameObject playerBarracks; // Renderer for displaying sprites
    public int barrackIndex = 0; // Index of the current barracks in the list

    private int currentUpgradeLevel = 0; // Current upgrade level
    private int currentDamageState = 0; // Current damage state
    public int health;
    private int startingHealth;

    private void Start()
    {
        if (barracksData == null || playerBarracks.GetComponent<SpriteRenderer>() == null) {
            Debug.LogError("BarracksData or SpriteRenderer is not assigned!");
            return;
        }

        // Determine if this is a player or enemy barracks based on tag
        if (CompareTag("PlayerBarracks")) {
            //Debug.Log("This is the Player's Barracks.");
        } else if (CompareTag("EnemyBarracks")) {
            //Debug.Log("This is the Enemy's Barracks.");
        } else {
            Debug.LogError("Barracks does not have a valid tag!");
        }

        barrackIndex = SaveManager.Instance.gameData.barracksUpgradeIndex;
        health = barracksData.barracks[barrackIndex].health;
        //Debug.Log($"Barracks health set to {health}.");
        UpdateSprite(); // Display the initial sprite

        startingHealth = health;
    }

    public void TakeDamage(int damage)
    {
        // Reduce current health by the damage amount
        health -= damage;

        // Calculate health percentage remaining
        float healthPercentage = (float)health / startingHealth;

        // Determine which sprite to show based on the health percentage
        int damageSpriteIndex = Mathf.FloorToInt((1 - healthPercentage) * (barracksData.barracks[barrackIndex].damageSprites.Length - 1));

        // Clamp the index to ensure it stays within the valid range of sprites
        damageSpriteIndex = Mathf.Clamp(damageSpriteIndex, 0, barracksData.barracks[barrackIndex].damageSprites.Length - 1);

        // Update the sprite if it's different from the current one
        if (currentDamageState != damageSpriteIndex) {
            currentDamageState = damageSpriteIndex;
            UpdateSprite();
        }

        // Check if the barracks are destroyed
        if (health <= 0) {
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
