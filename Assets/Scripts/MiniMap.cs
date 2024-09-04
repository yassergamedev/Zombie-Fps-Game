using UnityEngine;
using System.Collections.Generic;

public class Minimap : MonoBehaviour
{
    public Transform player;           // Reference to the player transform
    public GameObject playerIconPrefab; // Prefab for the player icon
    public GameObject doorIconPrefab;   // Prefab for the door icon
    public GameObject enemyIconPrefab;  // Prefab for the enemy icon

    public Camera minimapCamera;       // Camera rendering the minimap
    public RectTransform minimapRect;  // Reference to the minimap RectTransform

    private GameObject playerIcon;     // Instance of the player icon
    private List<GameObject> doorIcons = new List<GameObject>();  // List to store the door icons
    private List<Transform> doors = new List<Transform>();        // List to store the door transforms

    private List<GameObject> enemyIcons = new List<GameObject>(); // List to store the enemy icons
    private List<Transform> enemies = new List<Transform>();      // List to store the enemy transforms

    private void Start()
    {
        // Instantiate the player icon and parent it to the minimap
        playerIcon = Instantiate(playerIconPrefab, minimapRect);

        // Find all doors in the scene (assuming they have a "Door" tag)
        GameObject[] doorObjects = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in doorObjects)
        {
            doors.Add(door.transform);

            // Instantiate and store the door icons
            GameObject doorIcon = Instantiate(doorIconPrefab, minimapRect);
            doorIcons.Add(doorIcon);
        }

        // Find all zombies in the scene
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Zombie");
        foreach (GameObject enemy in enemyObjects)
        {
            enemies.Add(enemy.transform);

            // Instantiate and store the enemy icons
            GameObject enemyIcon = Instantiate(enemyIconPrefab, minimapRect);
            enemyIcons.Add(enemyIcon);
        }
    }

    private void Update()
    {
        // Update the player's icon position
        UpdateIconPosition(player, playerIcon);

        // Update the door icons' positions
        for (int i = 0; i < doors.Count; i++)
        {
            UpdateIconPosition(doors[i], doorIcons[i]);
        }

        // Update the enemies' icons positions
        for (int i = 0; i < enemies.Count; i++)
        {
            UpdateIconPosition(enemies[i], enemyIcons[i]);
        }
    }

    private void UpdateIconPosition(Transform target, GameObject icon)
    {
        // Calculate the local position relative to the player
        Vector3 offset = target.position - player.position;

        // Rotate the offset according to the player's rotation
        offset = Quaternion.Euler(0, -player.eulerAngles.y, 0) * offset;

        // Calculate the position relative to the minimap's size
        float minimapSize = minimapRect.rect.width;
        Vector3 iconPos = new Vector3(offset.x, offset.z, 0) / minimapSize * (minimapSize / 2f);

        // Update the icon's position on the minimap
        icon.transform.localPosition = iconPos;
    }
}
