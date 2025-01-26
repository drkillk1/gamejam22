using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab; // The bullet to shoot
    public Transform gunOffset; // The position to shoot from
    public float minChargeTime = 0.2f; // Minimum charge time for a small bullet
    public float maxChargeTime = 1.5f; // Maximum charge time for the largest bullet
    public float minBulletSpeed = 5f; // Slowest bullet speed (when fully charged)
    public float maxBulletSpeed = 20f; // Fastest bullet speed (when minimally charged)
    public float bulletSizeMultiplier = 5f; // Maximum size multiplier for the bullet
    public float initialBulletSize = 0.3f; // Initial size of the bullet when it spawns
    public Slider chargeBar; // Optional UI slider for the charge bar
    public Image chargeBarFill; // Image component for the charge bar fill color

    private bool isCharging = false; // Whether the player is holding the fire button
    private float chargeStartTime = 0f; // When the charging started
    private GameObject currentBullet = null; // Reference to the growing bullet

    void Start()
    {
        if (chargeBar != null)
        {
            chargeBar.minValue = 0f;
            chargeBar.maxValue = maxChargeTime;
            chargeBar.value = 0f;
        }
    }

    void Update()
    {
        if (isCharging)
        {
            float chargeTime = Mathf.Min(Time.time - chargeStartTime, maxChargeTime); // Cap at max charge time

            // Update the charge bar
            if (chargeBar != null)
            {
                chargeBar.value = Mathf.Clamp(chargeTime, 0f, maxChargeTime);
                UpdateChargeBarColor(chargeTime);
            }

            // Update the size and position of the bullet if it exists
            if (currentBullet != null)
            {
                UpdateBulletSize(chargeTime);
                UpdateBulletPosition();
            }
        }
    }

    private void FireBullet(float chargeTime)
    {
        if (currentBullet != null)
        {
            float chargeRatio = Mathf.Clamp01((chargeTime - minChargeTime) / (maxChargeTime - minChargeTime));
            float bulletSpeed = Mathf.Lerp(maxBulletSpeed, minBulletSpeed, chargeRatio); // Speed inversely proportional to charge

            // Mark the bullet as fired
            Bubble bubble = currentBullet.GetComponent<Bubble>();
            if (bubble != null)
            {
                bubble.SetIsCharged(true); // Set the bullet to a "fired" state
            }

            // Shoot the bullet
            Rigidbody2D rb = currentBullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.isKinematic = false; // Allow physics-based movement
                rb.velocity = bulletSpeed * gunOffset.up; // Shoot the bullet
            }

            currentBullet = null; // Reset the current bullet reference
        }

        // Reset the charge bar
        if (chargeBar != null)
        {
            chargeBar.value = 0f;
        }
    }




    private void UpdateChargeBarColor(float chargeTime)
    {
        if (chargeBarFill != null)
        {
            float chargeRatio = chargeTime / maxChargeTime;
            Color newColor = Color.Lerp(Color.green, Color.red, chargeRatio); // Green to red transition
            chargeBarFill.color = newColor;
        }
    }

    private void UpdateBulletSize(float chargeTime)
{
    float chargeRatio = Mathf.Clamp01(chargeTime / maxChargeTime);
    float bulletSize = Mathf.Lerp(initialBulletSize, bulletSizeMultiplier, Mathf.Pow(chargeRatio, 2)); // Exponential growth

    // Set the size of the bullet
    currentBullet.transform.localScale = Vector3.one * bulletSize;

    // Check if the bullet reaches maximum size
    if (chargeTime >= maxChargeTime)
    {
        // "Pop" the bullet (destroy it)
        Destroy(currentBullet);
        currentBullet = null;
        Debug.Log("Bullet popped! Charge too long.");
    }
}


    private void UpdateBulletPosition()
    {
        if (currentBullet != null && gunOffset != null)
        {
            currentBullet.transform.position = gunOffset.position;
            currentBullet.transform.rotation = gunOffset.rotation;
        }
    }

    private void OnFire(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            // Start charging
            isCharging = true;
            chargeStartTime = Time.time;

            // Instantiate the bullet in front of the player
            if (currentBullet == null)
            {
                currentBullet = Instantiate(bulletPrefab, gunOffset.position, gunOffset.rotation);

                Rigidbody2D rb = currentBullet.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.isKinematic = true; // Prevent the bullet from moving while charging
                }

                // Start with a smaller bullet size
                currentBullet.transform.localScale = Vector3.one * initialBulletSize;
            }
        }
        else
        {
            // Release the fire button
            if (isCharging)
            {
                isCharging = false;

                float chargeTime = Mathf.Min(Time.time - chargeStartTime, maxChargeTime); // Cap at max charge time

                if (currentBullet != null && chargeTime <= maxChargeTime)
                {
                    // Shoot the bullet
                    FireBullet(chargeTime);
                }
                else
                {
                    // Destroy the bullet if it's still present but not shot
                    if (currentBullet != null)
                    {
                        Destroy(currentBullet);
                        Debug.Log("Bullet not fired and destroyed.");
                    }
                }
            }
        }
    }
}
