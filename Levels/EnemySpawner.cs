using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval; // Time between spawns
    [SerializeField] private int maxEnemiesPerWave; // Number of enemies per wave

    [Header("Spawning State")]
    [SerializeField] private int enemiesSpawned = 0;
    [SerializeField] private bool spawning = false;

    [Header("Level Data")]
    [SerializeField] private LevelData currentLevel;
    [SerializeField] private LevelDifficulty currentDifficulty;

    [Header("Coroutine")]
    [SerializeField] private Coroutine spawnCoroutine;

    private void Start()
    {
        currentLevel = GameManager.Instance.currentLevel;
        currentDifficulty = GameManager.Instance.GetCurrentLevelDifficulty();
        maxEnemiesPerWave = currentDifficulty.maxEnemies;
        spawnInterval = Mathf.Max(1f, 3f - ((currentLevel.levelIndex + 1) * 0.2f));

        StartSpawning();
    }

    public void StartSpawning()
    {
        if (!spawning) {
            spawning = true;
            spawnCoroutine = StartCoroutine(SpawnEnemies());
        }
    }

    public void StopSpawning()
    {
        if (spawning) {
            spawning = false;
            if (spawnCoroutine != null) {
                StopCoroutine(spawnCoroutine); // Stop the specific coroutine
                spawnCoroutine = null;
            }
        }
    }

    private IEnumerator SpawnEnemies()
    {
        enemiesSpawned = 0; // Reset spawned enemy count

        while (spawning) {
            if (enemiesSpawned < maxEnemiesPerWave) {
                // Spawn an enemy and increment the counter
                SpawnEnemy();
                enemiesSpawned++;

                // Wait for the interval before spawning the next enemy
                yield return new WaitForSeconds(spawnInterval);
            } else {
                StopSpawning();
                break; // Prevent infinite loop
            }
        }
    }



    private void SpawnEnemy()
    {
        // Use the spawner to spawn an enemy based on the current level and max enemy index
        FindAnyObjectByType<Spawner>().TrySpawnEnemyCharacter(currentLevel.maxEnemyIndex);
    }


    public void ResetSpawner()
    {
        enemiesSpawned = 0;
    }
}
