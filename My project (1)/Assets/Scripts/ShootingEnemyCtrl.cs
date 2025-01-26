using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingEnemyCtrl : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public float maxHealth = 50f; // Maximum health for the enemy
    private float currentHealth;

    private Rigidbody2D rb;
    private Transform playerTransform;

    public GameObject bulletPrefab; // Prefab for the enemy's bullets
    public Transform firePoint; // The point from which the bullets are fired
    public float bulletSpeed = 10f; // Speed of the bullets
    public float shootingInterval = 2f; // Time between each shot

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
        StartCoroutine(ShootAtPlayer());
    }

    void Update()
    {
        if (!isAlive) return;

        MoveTowardsPlayer();
        UpdateHealthBarPosition();
    }

    private void MoveTowardsPlayer()
    {
        if (playerTransform == null) return;

        Vector2 targetDirection = (playerTransform.position - transform.position).normalized;

        // Rotate towards the player
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, targetDirection);
        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        rb.SetRotation(rotation);

        // Move towards the player
        rb.velocity = transform.up * speed;
    }

    private IEnumerator ShootAtPlayer()
    {
        while (isAlive)
        {
            Debug.Log("Shooting at player...");
            if (playerTransform != null && bulletPrefab != null && firePoint != null)
            {
                // Instantiate and shoot the bullet
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                if (bulletRb != null)
                {
                    Vector2 direction = (playerTransform.position - firePoint.position).normalized;
                    bulletRb.velocity = direction * bulletSpeed;
                }
            }

            yield return new WaitForSeconds(shootingInterval); // Wait before shooting again
        }

        Debug.Log("Shooting coroutine ended.");
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Shooting Enemy Health: {currentHealth}");

        if (healthBar != null)
        {
            healthBar.value = currentHealth; // Update health bar
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Calling Die() method...");
            Die();
        }
    }

    private void Die()
    {
        if (!isAlive) return; // Prevent multiple calls to Die()
        
        isAlive = false; // Mark the enemy as dead
        Debug.Log("Shooting Enemy Died!");
        StopAllCoroutines(); // Stop the shooting coroutine

        // Destroy the health bar if it exists
        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
        }

        // Destroy the enemy GameObject
        Destroy(gameObject);
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
