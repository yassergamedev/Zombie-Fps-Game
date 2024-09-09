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
    public int remainingZombies = 0;
    private int deathCounter = 0; // Counter to track the number of zombie deaths

    private bool isTimeStopped = false; // Flag to check if time stop is active

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Only start a new wave if not currently spawning and no zombies remaining
        if (!spawningWave && remainingZombies <= 0 && !isTimeStopped)
        {
            StartCoroutine(StartNextWave());

            // Track the highest wave the player has reached
            int highestWave = PlayerPrefs.GetInt("WaveMax", 0);
            if (currentWaveIndex > highestWave)
            {
                PlayerPrefs.SetInt("WaveMax", currentWaveIndex);
                PlayerPrefs.Save();
            }
        }
    }

    IEnumerator StartNextWave()
    {
        currentWaveIndex++;
        spawningWave = true;
        UpdateWaveNumberUI();

        if (audioSource != null && waveStartSound != null)
        {
            audioSource.PlayOneShot(waveStartSound);
        }

        yield return new WaitForSeconds(timeBetweenWaves); // Wait before starting wave

        int zombieCount = Mathf.Min(Mathf.RoundToInt(initialZombieCount * Mathf.Pow(spawnRateMultiplier, currentWaveIndex)), maxZombiesPerWave);
        remainingZombies = zombieCount;

        for (int i = 0; i < zombieCount; i++)
        {
            // Pause spawning if time stop is active
            while (isTimeStopped)
            {
                
                yield return null; // Wait until time stop ends
            }

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            SpawnZombie(zombiePrefabs[(int)Random.Range(0, zombiePrefabs.Length)], spawnPoint.position, spawnPoint.rotation);
            yield return new WaitForSeconds(spawnInterval);
        }

        spawningWave = false;
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

            StartCoroutine(StartNextWave());
        }
    }

    void UpdateWaveNumberUI()
    {
        if (waveNumberText != null)
        {
            waveNumberText.text = (currentWaveIndex).ToString();
            waveNumberText.gameObject.GetComponent<Animator>().SetTrigger("Flicker");
        }
    }

    // Method to change spawn points based on player location
    public void UpdateSpawnPoints(Transform[] newSpawnPoints, GameObject[] enemies)
    {
        if (newSpawnPoints.Length > 0)
        {
            spawnPoints = newSpawnPoints;
            zombiePrefabs = enemies;
        }
    }

    // Method to set isTimeStopped flag (to be called by TimeStop script)
    public void SetTimeStop(bool value)
    {
        isTimeStopped = value;
    }
}
