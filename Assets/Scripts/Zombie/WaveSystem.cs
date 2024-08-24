using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int aggressiveZombieCount;
        public int passiveZombieCount;
    }

    public Wave[] waves; // Array of waves, each with a number of zombies
    public Transform[] spawnPoints; // Array of spawn points
    public GameObject aggressiveZombiePrefab; // Prefab for the aggressive zombie
    public GameObject passiveZombiePrefab; // Prefab for the passive zombie
    public float timeBetweenWaves = 10f; // Time between each wave
    public float spawnInterval = 1f; // Time between each zombie spawn

    private int currentWaveIndex = 0;
    private bool spawningWave = false;

    void Update()
    {
        if (!spawningWave && currentWaveIndex < waves.Length)
        {
            StartCoroutine(SpawnWave(waves[currentWaveIndex]));
            currentWaveIndex++;
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        spawningWave = true;

        for (int i = 0; i < wave.aggressiveZombieCount; i++)
        {
            SpawnZombie(aggressiveZombiePrefab, "Run");
            yield return new WaitForSeconds(spawnInterval);
        }

        for (int i = 0; i < wave.passiveZombieCount; i++)
        {
            SpawnZombie(passiveZombiePrefab, "Walk");
            yield return new WaitForSeconds(spawnInterval);
        }

        yield return new WaitForSeconds(timeBetweenWaves);
        spawningWave = false;
    }

    void SpawnZombie(GameObject zombiePrefab, string startAnimation)
    {
        // Randomly select a spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Instantiate the zombie at the spawn point
        GameObject zombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);

        // Set the starting animation
        Zombie zombieScript = zombie.GetComponent<Zombie>();
        zombieScript.zombieAnimator.Play(startAnimation);
    }
}
