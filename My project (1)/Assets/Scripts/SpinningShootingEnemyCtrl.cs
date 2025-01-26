using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinningShootingEnemyCtrl : MonoBehaviour
{
    public float speed; // Speed at which the enemy moves
    public float rotationSpeed = 360f; // Degrees per second for spinning
    public float originalSpeed = 5f;
    public float originalRotationSpeed = 360f;
    public float maxHealth = 50f; // Maximum health for the enemy
    public float currentHealth;

    private Rigidbody2D rb;
    private Transform playerTransform;

    public GameObject bulletPrefab; // Prefab for the enemy's bullets
    public Transform firePoint; // The point from which the bullets are fired
    public float bulletSpeed = 10f; // Speed of the bullets
    public float shootingInterval = 1f; // Time between each shot

    public Slider healthBar; // Health bar slider attached to the enemy
    public Vector3 healthBarOffset = new Vector3(0, 1.5f, 0); // Offset for the health bar above the enemy
    private Quaternion fixedRotation; // Fixed rotation for the health bar

    private bool isAlive = true; // Tracks if the enemy is alive

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth; // Initialize enemy health

        // Find the player's Transform
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
            fixedRotation = healthBar.transform.rotation; // Store the initial rotation
        }
    }

    void Start()
    {
        // Start shooting bullets
        StartCoroutine(ShootWhileSpinning());
    }

    void Update()
    {
        if (!isAlive) return;

        ChasePlayer();
        UpdateHealthBarPosition();
    }

    private void ChasePlayer()
    {
        if (playerTransform == null) return;

        // Move toward the player
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.velocity = direction * speed;

        // Spin the enemy
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    private bool canShoot = true; // Controls whether the enemy can shoot

    private IEnumerator ShootWhileSpinning()
    {
        while (isAlive)
        {
            if (canShoot && bulletPrefab != null && firePoint != null)
            {
                // Instantiate and shoot the bullet
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                if (bulletRb != null)
                {
                    bulletRb.velocity = firePoint.up * bulletSpeed;
                }
            }

            yield return new WaitForSeconds(shootingInterval); // Wait before shooting again
        }
    }

    public void SetCanShoot(bool value)
    {
        canShoot = value;
        Debug.Log($"SpinningShootingEnemy canShoot set to: {canShoot}");
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Spinning Shooting Enemy Health: {currentHealth}");

        if (healthBar != null)
        {
            healthBar.value = currentHealth; // Update health bar
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Freeze(bool isFrozen)
    {
        if (isFrozen)
        {
            SetCanShoot(false);
            speed = 0f; // Stop movement
            rotationSpeed = 0f; // Stop spinning
            Debug.Log("Spinning shooting enemy frozen!");
        }
        else
        {
            SetCanShoot(true);
            speed = originalSpeed; // Restore movement speed
            rotationSpeed = originalRotationSpeed; // Restore spinning speed
            Debug.Log("Spinning shooting enemy unfrozen!");
        }
    }

    
    public void ApplyBurnEffect(float totalDamage, int burnTicks, float tickInterval)
    {
        StartCoroutine(TakeBurnDamage(totalDamage, burnTicks, tickInterval));
    }

    private IEnumerator TakeBurnDamage(float totalDamage, int burnTicks, float tickInterval)
    {
        Debug.Log($"Shooting enemy burning for {totalDamage} total damage over {burnTicks} ticks.");
        float damagePerTick = totalDamage / burnTicks;

        for (int i = 0; i < burnTicks; i++)
        {
            if (currentHealth <= 0)
            {
                Debug.Log("Shooting enemy already dead during burn effect. Stopping burn.");
                yield break;
            }

            TakeDamage(damagePerTick);
            Debug.Log($"Burn tick {i + 1}/{burnTicks}: {damagePerTick} damage applied. Current health: {currentHealth}");

            yield return new WaitForSeconds(tickInterval); // Wait for the next tick
        }

        Debug.Log("Shooting enemy burn effect ended.");
    }


    private void Die()
    {
        isAlive = false; // Mark the enemy as dead
        Debug.Log("Spinning Shooting Enemy Died!");
        StopAllCoroutines(); // Stop all coroutines
        Destroy(gameObject); // Destroy the enemy
    }

    private void UpdateHealthBarPosition()
    {
        if (healthBar != null)
        {
            // Position the health bar above the enemy
            healthBar.transform.position = transform.position + healthBarOffset;

            // Keep the health bar rotation fixed
            healthBar.transform.rotation = fixedRotation;
        }
    }
}
