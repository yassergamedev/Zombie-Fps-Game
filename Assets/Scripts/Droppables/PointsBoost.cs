using System.Collections;
using UnityEngine;

public class PointsBoost : MonoBehaviour
{
    public float pointsBoostDuration = 10f;
    public int pointsMultiplier = 2; // The factor by which points will be multiplied

    private WeaponController weaponController;
    private PlayerUI playerUI;
    private int originalPointsOnHit;

    void Start()
    {
        weaponController = FindObjectOfType<WeaponController>();
        playerUI = FindObjectOfType<PlayerUI>();

        if (weaponController == null || playerUI == null)
        {
            Debug.LogError("WeaponController or PlayerUI not found in the scene.");
        }
    }

    public void ActivatePointsBoost()
    {
        if (weaponController != null && playerUI != null)
        {


            playerUI.StartTimer(pointsBoostDuration, "Points Boost");
            playerUI.StartPointsBoostTimer(pointsBoostDuration, pointsMultiplier);
        }
    }

   
}
