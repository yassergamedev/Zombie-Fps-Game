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

    private List<Image> bulletImages = new List<Image>();

    private Coroutine instaKillCoroutine;
    private Coroutine pointsBoostCoroutine;
    private Coroutine timeStopCoroutine;


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

        if (isPaused)
        {
            ResumeTimers();
        }
        weaponController = GetComponent<WeaponController>();
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
        text.text = "+" + pointsAdded.ToString();
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
        if (instaKillCoroutine != null)
        {
            StopCoroutine(instaKillCoroutine);
        }

        instaKillTimeRemaining = duration;
        originalDamage = weaponController.currentWeapon.damage;
        weaponController.currentWeapon.damage = instaKillDamage;

        instaKillCoroutine = StartCoroutine(InstaKillCountdown());
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

        weaponController.currentWeapon.damage = originalDamage;
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
        if (instaKillTimerText != null)
        {
            instaKillTimerText.gameObject.SetActive(true);
            StartCoroutine(UpdateTimer(duration, timer));
        }
    }

    private IEnumerator UpdateTimer(float duration, string timer)
    {
        float timeRemaining = duration;

        while (timeRemaining > 0)
        {
            if (!isPaused)
            {
                instaKillTimerText.text = timer + " : " + timeRemaining.ToString("F1") + "s";
                timeRemaining -= Time.deltaTime;
            }
            yield return null;
        }

        instaKillTimerText.gameObject.SetActive(false);
    }

}
