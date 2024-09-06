using System.Collections;
using UnityEngine;

public class RandomWeapon : Interactable
{
    public GameObject[] weaponPrefabs; // Array of weapon prefabs to shuffle through
    public float shuffleDuration = 3f; // Duration of the shuffling process
    public float shuffleInterval = 0.2f; // Time between switching weapons during the shuffle
    public AudioClip shuffleSound; // Sound effect played during shuffling
    public AudioClip selectedSound; // Sound effect played when a weapon is selected

    private AudioSource audioSource; // Audio source for playing sounds
    private int selectedWeaponIndex; // Index of the selected weapon
    private bool isShuffling = false; // To prevent interaction during shuffling
    private WeaponController weaponController;
    public AmmoBox ammoBox;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        weaponController = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponController>();
        StartCoroutine(ShuffleWeapons());
        if (weaponController == null)
        {
            Debug.LogError("WeaponController not found in the scene.");
        }
    }

 

    protected override void Interact()
    {
        if (!isShuffling)
        {
            weaponController.SwitchWeapon(selectedWeaponIndex);
            transform.parent.GetChild(0).gameObject.GetComponent<AmmoBox>().Close();
            Destroy(this.gameObject);
        }
    }

    public IEnumerator ShuffleWeapons()
    {
        isShuffling = true;
        float elapsedTime = 0f;
        int currentIndex = 0;

        
        audioSource.Play();
        weaponPrefabs[1].SetActive(true);
        while (elapsedTime < shuffleDuration)
        {
            // Cycle through the weapons
            currentIndex = (currentIndex + 1) % weaponPrefabs.Length;

            // Show the current weapon (e.g., change the visible weapon model)
            DisplayWeapon(currentIndex);

            elapsedTime += shuffleInterval;
            yield return new WaitForSeconds(shuffleInterval);
        }

        

        // Randomly select a weapon
        selectedWeaponIndex = Random.Range(0, weaponPrefabs.Length);
        DisplayWeapon(selectedWeaponIndex); // Display the selected weapon

        isShuffling = false;
    }

    private void DisplayWeapon(int weaponIndex)
    {
        // Logic to display the weapon (e.g., activating a GameObject)
        for (int i = 0; i < weaponPrefabs.Length; i++)
        {
            if(i == weaponIndex)
            {
                weaponPrefabs[i].SetActive(true);

            }
            else
            {
                weaponPrefabs[i].SetActive(false);
            }
            
        }
    }
}
