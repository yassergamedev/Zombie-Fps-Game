using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI ammoCountText;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform ammoPanel;
    [SerializeField] private Color bulletColorFull = Color.white;
    [SerializeField] private Color bulletColorEmpty = Color.gray;

    [SerializeField] private GameObject pointsTextPrefab;
    [SerializeField] private Transform pointsTextParent;

    [SerializeField] private TextMeshProUGUI instaKillTimerText; // Text for InstaKill timer
    [SerializeField] private TextMeshProUGUI pointsBoostTimerText; // Text for InstaKill timer
    [SerializeField] private TextMeshProUGUI timeStopTimerText; // Text for InstaKill timer

    private List<Image> bulletImages = new List<Image>();

    private Coroutine instaKillCoroutine;
    private Coroutine pointsBoostCoroutine;
    private Coroutine timeStopCoroutine;
    private WaveSystem waveSystem;
    float[] originalDamages;
   [SerializeField] private bool isPaused = false;

    // Store the remaining time

    [SerializeField] private float instaKillTimeRemaining;
    [SerializeField] private float pointsBoostTimeRemaining;
    [SerializeField] private float timeStopTimeRemaining;
    private WeaponController weaponController;

    [SerializeField] private float originalDamage;
    [SerializeField] private int originalPointsOnHit;    
    void Start()
    {

        promptText.text = "";
        pointsText.text = "0";
        ammoCountText.text = "";

        if (instaKillTimerText != null)
        {
            instaKillTimerText.gameObject.SetActive(false); // Ensure InstaKill timer is hidden initially
        }
        if (pointsBoostTimerText != null)
        {
            pointsBoostTimerText.gameObject.SetActive(false); // Ensure pointsBoost timer is hidden initially
        }
        if (timeStopTimerText != null)
        {
            timeStopTimerText.gameObject.SetActive(false); // Ensure timeStop timer is hidden initially
        }


        if (isPaused)
        {
            ResumeTimers();
        }
        weaponController = GetComponent<WeaponController>();
        waveSystem = FindObjectOfType<WaveSystem>();
        if (waveSystem == null)
        {
            Debug.LogError("WaveSystem not found in the scene.");
        }
    }

    public void UpdateText(string promptMessage)
    {
        promptText.text = promptMessage;
    }

    public void UpdatePointsText(int points, int pointsAdded)
    {
        pointsText.text = points.ToString();
        AnimatePointsText(pointsAdded);
    }

    public void UpdateAmmoText(int currentAmmo, int maxAmmo)
    {
        while (bulletImages.Count < currentAmmo)
        {
            GameObject newBullet = Instantiate(bulletPrefab, ammoPanel);
            bulletImages.Add(newBullet.GetComponent<Image>());
        }

        while (bulletImages.Count > currentAmmo)
        {
            Destroy(bulletImages[bulletImages.Count - 1].gameObject);
            bulletImages.RemoveAt(bulletImages.Count - 1);
        }

        for (int i = 0; i < bulletImages.Count; i++)
        {
            bulletImages[i].color = i < currentAmmo ? bulletColorFull : bulletColorEmpty;
        }

        ammoCountText.text = $"{currentAmmo} / {maxAmmo}";
    }

    private void AnimatePointsText(int pointsAdded)
    {
        Vector3 customPosition = new Vector3(-93, 24, 0);
        GameObject animText = Instantiate(pointsTextPrefab, pointsTextParent);
        animText.transform.localPosition = customPosition;

        TextMeshProUGUI text = animText.GetComponent<TextMeshProUGUI>();
        text.text =  pointsAdded.ToString();
        text.color = Color.yellow;

        animText.GetComponent<Animator>().Play("PointsAddedAnimation");
        Destroy(animText, 2f);
    }

    public void UpdateBulletCount(int newBulletCount)
    {
        UpdateAmmoText(newBulletCount, bulletImages.Count);
    }

    // New methods to control the InstaKill timer UI




    public void StartInstaKillTimer(float duration, int instaKillDamage)
    {
        instaKillTimeRemaining = duration;

       
       originalDamages = new float[weaponController.weapons.Count];

      
        for (int i = 0; i < weaponController.weapons.Count; i++)
        {
           
            originalDamages[i] = weaponController.weapons[i].GetComponent<Weapon>().damage;

            
            weaponController.weapons[i].GetComponent<Weapon>().damage = instaKillDamage;
        }

       
        StartCoroutine(InstaKillCountdown());

        
        StartTimer(duration, "InstaKill");
    }


    private IEnumerator InstaKillCountdown()
    {
        while (instaKillTimeRemaining > 0)
        {
            if (!isPaused)
            {
                instaKillTimeRemaining -= Time.deltaTime;
            }
            yield return null;
        }

       
        // After the timer, reset the damage values to their original values
        for (int i = 0; i < weaponController.weapons.Count; i++)
        {
            weaponController.weapons[i].GetComponent<Weapon>().damage = originalDamages[i];
        }
    }

    public void StartPointsBoostTimer(float duration, int pointsMultiplier)
    {
        if (pointsBoostCoroutine != null)
        {
            StopCoroutine(pointsBoostCoroutine);
        }

        pointsBoostTimeRemaining = duration;
        originalPointsOnHit = weaponController.currentWeapon.pointsOnHit;
        weaponController.currentWeapon.pointsOnHit *= pointsMultiplier;

        pointsBoostCoroutine = StartCoroutine(PointsBoostCountdown());
        StartTimer(duration, "Points Boost");
    }

    private IEnumerator PointsBoostCountdown()
    {
        while (pointsBoostTimeRemaining > 0)
        {
            if (!isPaused)
            {
                pointsBoostTimeRemaining -= Time.deltaTime;
            }
            yield return null;
        }

        weaponController.currentWeapon.pointsOnHit = originalPointsOnHit;
    }
    public void StartTimeStopTimer(float duration)
    {
        if (timeStopCoroutine != null)
        {
            StopCoroutine(timeStopCoroutine);
        }

        timeStopTimeRemaining = duration;
        FreezeZombies();

        timeStopCoroutine = StartCoroutine(TimeStopCountdown());
        StartTimer(duration, "Time Stop");
    }

    private IEnumerator TimeStopCountdown()
    {
        while (timeStopTimeRemaining > 0)
        {
            if (!isPaused)
            {
                timeStopTimeRemaining -= Time.deltaTime;
            }
            yield return null;
        }
        //  UnPause the spawning in WaveSystem
        waveSystem.SetTimeStop(false);
        UnfreezeZombies();
    }

    private void FreezeZombies()
    {
        Zombie[] zombies = FindObjectsOfType<Zombie>();
        foreach (Zombie zombie in zombies)
        {
            zombie.Freeze(); // You need to implement the Freeze method in your Zombie script
        }
    }

    private void UnfreezeZombies()
    {
        Zombie[] zombies = FindObjectsOfType<Zombie>();
        foreach (Zombie zombie in zombies)
        {
            zombie.Unfreeze(); // You need to implement the Unfreeze method in your Zombie script
        }
    }
    public void PauseTimers()
    {
        isPaused = true;
    }

    public void ResumeTimers()
    {
        isPaused = false;

        if (instaKillTimeRemaining > 0 )
        {
            instaKillCoroutine = StartCoroutine(InstaKillCountdown());
            StartCoroutine(UpdateTimer(instaKillTimeRemaining,"InstaKill"));
        }

        if (pointsBoostTimeRemaining > 0)
        {
            pointsBoostCoroutine = StartCoroutine(PointsBoostCountdown());
            StartCoroutine(UpdateTimer(pointsBoostTimeRemaining, "Points Boost"));
        }
        if (timeStopTimeRemaining > 0 )
        {
            timeStopCoroutine = StartCoroutine(TimeStopCountdown());
            StartCoroutine(UpdateTimer(pointsBoostTimeRemaining, "Time Stop"));
        }
    }

    public void StartTimer(float duration, string timer)
    {
       

        switch (timer)
        {
            case "InstaKill":
                {
                    instaKillTimerText.gameObject.SetActive(true);
                    
                    break;
                }
            case "Points Boost":
                {
                    pointsBoostTimerText.gameObject.SetActive(true);
                   
                    break;
                }
            case "Time Stop":
                {
                    timeStopTimerText.gameObject.SetActive(true);
                    // Pause the spawning in WaveSystem
                    waveSystem.SetTimeStop(true);
                    break;
                }
        }
        StartCoroutine(UpdateTimer(duration, timer));
    }

    private IEnumerator UpdateTimer(float duration, string timer)
    {
        float timeRemaining = duration;

        while (timeRemaining > 0)
        {
            if (!isPaused)
            {
                switch (timer){
                    case "InstaKill":
                        {
                            instaKillTimerText.text = timer + " : " + instaKillTimeRemaining.ToString("F1") + "s";
                           
                            break;
                        }
                    case "Points Boost":
                        {
                            pointsBoostTimerText.text = timer + " : " + pointsBoostTimeRemaining.ToString("F1") + "s";
                           
                            break;
                        }
                    case "Time Stop":
                        {
                            timeStopTimerText.text = timer + " : " + timeStopTimeRemaining.ToString("F1") + "s";
                            
                            break;
                        }
                }
                timeRemaining -= Time.deltaTime;

            }
            yield return null;
        }

        instaKillTimerText.gameObject.SetActive(false);
        timeStopTimerText.gameObject.SetActive(false);
        pointsBoostTimerText.gameObject.SetActive(false);
    }

}
