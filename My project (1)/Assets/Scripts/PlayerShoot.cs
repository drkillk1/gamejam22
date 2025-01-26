using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerShoot : MonoBehaviour
{
    public GameObject normalBubblePrefab; // Prefab for normal bubble
    public GameObject fireBubblePrefab; // Prefab for fire bubble
    public GameObject iceBubblePrefab; // Prefab for ice bubble
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
    private GameObject currentBubble = null; // Reference to the growing bubble

    public enum BubbleType { 
        Normal = 0, 
        Fire = 1, 
        Ice = 2 }
    public BubbleType currentBubbleType = BubbleType.Normal; // The currently selected bubble type

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
        HandleAbilitySwitching();

        if (isCharging)
        {
            float chargeTime = Mathf.Min(Time.time - chargeStartTime, maxChargeTime); // Cap at max charge time

            // Update the charge bar
            if (chargeBar != null)
            {
                chargeBar.value = Mathf.Clamp(chargeTime, 0f, maxChargeTime);
                UpdateChargeBarColor(chargeTime);
            }

            // Update the size and position of the bubble if it exists
            if (currentBubble != null)
            {
                UpdateBulletSize(chargeTime);
                UpdateBulletPosition();
            }
        }
    }

    private void HandleAbilitySwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentBubbleType = BubbleType.Normal;
            Debug.Log("Switched to Normal Bubble");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentBubbleType = BubbleType.Fire;
            Debug.Log("Switched to Fire Bubble");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentBubbleType = BubbleType.Ice;
            Debug.Log("Switched to Ice Bubble");
        }
    }


    private void FireBullet(float chargeTime)
    {
        if (currentBubble != null)
        {
            float chargeRatio = Mathf.Clamp01((chargeTime - minChargeTime) / (maxChargeTime - minChargeTime));
            float bulletSpeed = Mathf.Lerp(maxBulletSpeed, minBulletSpeed, chargeRatio); // Speed inversely proportional to charge

            // Mark the bubble as fired
            Bubble bubble = currentBubble.GetComponent<Bubble>();
            if (bubble != null)
            {
                bubble.SetIsCharged(true); // Set the bubble to a "fired" state
                bubble.bubbleType = (Bubble.BubbleType)currentBubbleType; // Assign the correct bubble type
                Debug.Log($"Fired a {bubble.bubbleType} bubble.");
            }

            // Shoot the bubble
            Rigidbody2D rb = currentBubble.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.isKinematic = false; // Allow physics-based movement
                rb.velocity = bulletSpeed * gunOffset.up; // Shoot the bullet
            }

            currentBubble = null; // Reset the current bubble reference
        }

        // Reset the charge bar
        if (chargeBar != null)
        {
            chargeBar.value = 0f;
        }
    }

    private GameObject GetBubblePrefab()
    {
        GameObject selectedPrefab = null;

        switch (currentBubbleType)
        {
            case BubbleType.Fire:
                selectedPrefab = fireBubblePrefab;
                break;
            case BubbleType.Ice:
                selectedPrefab = iceBubblePrefab;
                break;
            case BubbleType.Normal:
                selectedPrefab = normalBubblePrefab;
                break;
            default:
                Debug.LogWarning("Unknown bubble type!");
                break;
        }

        Debug.Log($"Selected prefab: {selectedPrefab?.name} for type: {currentBubbleType}");
        return selectedPrefab;
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
        currentBubble.transform.localScale = (Vector3.one * bulletSize) * .15f;

        // Check if the bullet reaches maximum size
        if (chargeTime >= maxChargeTime)
        {
            // "Pop" the bullet (destroy it)
            Destroy(currentBubble);
            currentBubble = null;
            Debug.Log("Bullet popped! Charge too long.");
        }
    }

    private void UpdateBulletPosition()
    {
        if (currentBubble != null && gunOffset != null)
        {
            currentBubble.transform.position = gunOffset.position;
            currentBubble.transform.rotation = gunOffset.rotation;
        }
    }

    private void OnFire(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            // Start charging
            isCharging = true;
            chargeStartTime = Time.time;

            // Instantiate the bubble in front of the player
            if (currentBubble == null)
            {
                GameObject bubblePrefab = GetBubblePrefab(); // Dynamically get the appropriate prefab
                if (bubblePrefab == null)
                {
                    Debug.LogWarning("No bubble prefab found for the current bubble type!");
                    return;
                }

                currentBubble = Instantiate(bubblePrefab, gunOffset.position, gunOffset.rotation);

                Rigidbody2D rb = currentBubble.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.isKinematic = true; // Prevent the bubble from moving while charging
                }

                // Start with a smaller bubble size
                currentBubble.transform.localScale = Vector3.one * initialBulletSize;
            }
        }
        else
        {
            // Release the fire button
            if (isCharging)
            {
                isCharging = false;

                float chargeTime = Mathf.Min(Time.time - chargeStartTime, maxChargeTime); // Cap at max charge time

                if (currentBubble != null && chargeTime <= maxChargeTime)
                {
                    // Shoot the bubble
                    FireBullet(chargeTime);
                }
                else
                {
                    // Destroy the bubble if it's still present but not shot
                    if (currentBubble != null)
                    {
                        Destroy(currentBubble);
                        Debug.Log("Bullet not fired and destroyed.");
                    }
                }
            }
        }
    }

}
