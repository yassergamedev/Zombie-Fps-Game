using UnityEngine;

public class SpawnAreaTrigger : MonoBehaviour
{
    public Transform[] areaSpawnPoints; // Spawn points for this area
    public GameObject[] EnemyPrefabs; // EnemyPrefabs
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WaveSystem waveSystem = FindObjectOfType<WaveSystem>();
            if (waveSystem != null)
            {
                waveSystem.UpdateSpawnPoints(areaSpawnPoints,EnemyPrefabs);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WaveSystem waveSystem = FindObjectOfType<WaveSystem>();
            if (waveSystem != null)
            {
                waveSystem.UpdateSpawnPoints(areaSpawnPoints, EnemyPrefabs);
            }
        }
    }
}
