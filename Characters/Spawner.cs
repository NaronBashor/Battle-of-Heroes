using UnityEngine;

public class Spawner : MonoBehaviour
{
    public CharacterDatabase playerDatabase;
    public CharacterDatabase enemyDatabase;
    public GameObject characterPrefab;
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;
    private int sortingOrderCounter = 0;

    private void Start()
    {
        ResetSortingOrderCounter();
        //TrySpawnPlayerCharacter();
        TrySpawnEnemyCharacter();
    }

    public void TrySpawnPlayerCharacter(string characterName)
    {
        var unlockedCharacters = SaveManager.Instance.gameData.characters.FindAll(c => c.isUnlocked);

        if (unlockedCharacters.Count == 0) {
            Debug.LogWarning("No unlocked player characters available to spawn!");
            return;
        }

        //int randomIndex = Random.Range(0, unlockedCharacters.Count);
        //string characterName = unlockedCharacters[randomIndex].characterName;

        CharacterData data = playerDatabase.GetCharacterByName(characterName);
        SpawnCharacter(data, playerSpawnPoint);
    }

    private void TrySpawnEnemyCharacter()
    {
        int randomIndex = Random.Range(0, enemyDatabase.enemyCharacters.Count);
        CharacterData data = enemyDatabase.enemyCharacters[randomIndex];
        SpawnCharacter(data, enemySpawnPoint);
    }

    private void SpawnCharacter(CharacterData data, Transform spawnPoint)
    {
        Vector3 spawnPosition = spawnPoint.position + new Vector3(0, data.yOffset, 0);
        GameObject character = Instantiate(characterPrefab, spawnPosition, Quaternion.identity);
        CharacterController controller = character.GetComponent<CharacterController>();
        controller.characterData = data;
        // Set the unique sorting order
        SetUniqueSortingOrder(character);

        // Increment the sorting order counter
        sortingOrderCounter++;
    }

    private void SetUniqueSortingOrder(GameObject character)
    {
        SpriteRenderer spriteRenderer = character.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) {
            // Assign a unique sorting order
            spriteRenderer.sortingOrder = sortingOrderCounter;
        }
    }

    private void ResetSortingOrderCounter()
    {
        sortingOrderCounter = 0;
    }
}
