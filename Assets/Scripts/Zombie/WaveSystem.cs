using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSystem : MonoBehaviour
{
    public int initialZombieCount = 10; // Initial number of zombies for the first wave
    public Transform[] spawnPoints; // Array of spawn points
    public GameObject[] zombiePrefabs; // Prefabs for different zombie types
    public float timeBetweenWaves = 10f; // Cooldown between each wave
    public float spawnInterval = 1f; // Time between each zombie spawn
    public AudioClip waveStartSound; // Sound effect for starting a wave
    public AudioClip waveCooldownSound; // Sound effect for cooldown between waves
    public TMP_Text waveNumberText; // TextMesh Pro Text for displaying the current wave number
    public float spawnRateMultiplier = 1.2f; // Multiplier for increasing difficulty
    public float startCooldown = 10f; // Cooldown before starting the first wave
    public int rareZombieStartWave = 10; // Wave index at which rare zombies start to appear
    public int maxZombiesPerWave = 100; // Maximum number of zombies per wave

    private int currentWaveIndex = 0;
    private bool spawningWave = false;
    private AudioSource audioSource;
    private int remainingZombies;
    private int deathCounter = 0; // Counter to track the number of zombie deaths

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateWaveNumberUI();
        StartCoroutine(StartWaveAfterCooldown());
    }

    void Update()
    {
        if (!spawningWave && remainingZombies <= 0)
        {
            StartCoroutine(StartNextWave());
        }
    }

    IEnumerator StartWaveAfterCooldown()
    {
        yield return new WaitForSeconds(startCooldown); // Wait before starting the first wave
        StartNextWave();
    }

    IEnumerator StartNextWave()
    {
        currentWaveIndex++;
        spawningWave = true;

        if (audioSource != null && waveStartSound != null)
        {
            audioSource.PlayOneShot(waveStartSound);
        }

        int zombieCount = Mathf.Min(Mathf.RoundToInt(initialZombieCount * Mathf.Pow(spawnRateMultiplier, currentWaveIndex)), maxZombiesPerWave);
        remainingZombies = zombieCount;

        for (int i = 0; i < zombiePrefabs.Length; i++)
        {
            if (currentWaveIndex >= rareZombieStartWave || i < zombiePrefabs.Length - 1)
            {
                int spawnCount = zombieCount / zombiePrefabs.Length;
                for (int j = 0; j < spawnCount; j++)
                {
                    Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                    SpawnZombie(zombiePrefabs[i], spawnPoint.position, spawnPoint.rotation);
                    yield return new WaitForSeconds(spawnInterval);
                }
            }
        }

        UpdateWaveNumberUI();
        spawningWave = false;

        if (audioSource != null && waveCooldownSound != null)
        {
            audioSource.PlayOneShot(waveCooldownSound);
        }

        yield return new WaitForSeconds(timeBetweenWaves);
    }

    void SpawnZombie(GameObject zombiePrefab, Vector3 position, Quaternion rotation)
    {
        GameObject zombie = Instantiate(zombiePrefab, position, rotation);
        Zombie zombieScript = zombie.GetComponent<Zombie>();

        if (zombieScript != null)
        {
            zombieScript.OnZombieKilled += OnZombieKilled;
        }
    }

    void OnZombieKilled()
    {
        deathCounter++;
        remainingZombies--;

        if (remainingZombies <= 0)
        {
            // Track the highest wave the player has reached
            int highestWave = PlayerPrefs.GetInt("WaveMax", 0);
            if (currentWaveIndex > highestWave)
            {
                PlayerPrefs.SetInt("WaveMax", currentWaveIndex);
                PlayerPrefs.Save();
            }

            StartNextWave();
        }
    }

    void UpdateWaveNumberUI()
    {
        if (waveNumberText != null)
        {
            waveNumberText.text = (currentWaveIndex + 1).ToString();
            waveNumberText.gameObject.GetComponent<Animator>().SetTrigger("Flicker");
        }
    }

    // Method to change spawn points based on player location
    public void UpdateSpawnPoints(Transform[] newSpawnPoints)
    {
        spawnPoints = newSpawnPoints;
    }
}
