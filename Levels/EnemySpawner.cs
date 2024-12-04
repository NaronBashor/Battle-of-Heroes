using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float spawnInterval; // Time between spawns
    public int maxEnemiesPerWave; // Number of enemies per wave

    private int enemiesSpawned = 0;
    private bool spawning = false;

    public LevelData currentLevel;
    LevelDifficulty currentDifficulty;

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
            StartCoroutine(SpawnEnemies());
        }
    }

    public void StopSpawning()
    {
        spawning = false;
        StopCoroutine(SpawnEnemies());
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
