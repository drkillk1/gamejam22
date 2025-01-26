using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCtrl : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public float maxHealth = 50f; // Maximum health for the enemy
    public float currentHealth;
    public float originalSpeed = 2f;

    public float freezeDuration = 2f;

    private Rigidbody2D rb;
    private PlayerAwarenessController playerAwarenessController;
    private Vector2 targetDirection;

    public float attackDamage = 50f; // Damage the enemy deals to the player

    public Slider healthBar; // Health bar slider attached to the enemy
    public Vector3 healthBarOffset = new Vector3(0, 1.5f, 0); // Offset for the health bar above the enemy
    private Quaternion fixedRotation; // Fixed rotation for the health bar

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAwarenessController = GetComponent<PlayerAwarenessController>();
        currentHealth = maxHealth; // Initialize enemy health
        originalSpeed = speed; // Store the original speed

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
            fixedRotation = healthBar.transform.rotation; // Store the initial rotation
        }
    }

    void Update()
    {
        UpdateTargetDirection();
        RotateTowardsTarget();
        SetVelocity();
        UpdateHealthBarPosition();
    }

    private void UpdateTargetDirection()
    {
        if (playerAwarenessController.AwareOfPlayer)
        {
            targetDirection = playerAwarenessController.DirectionToPlayer;
        }
        else
        {
            targetDirection = Vector2.zero;
        }
    }

    private void RotateTowardsTarget()
    {
        if (targetDirection == Vector2.zero)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, targetDirection);
        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        rb.SetRotation(rotation);
    }

    private void SetVelocity()
    {
        if (targetDirection == Vector2.zero)
        {
            rb.velocity = Vector2.zero;
        }
        else
        {
            rb.velocity = transform.up * speed;
        }
    }

    public void Freeze(bool isFrozen)
    {
        if (isFrozen)
        {
            speed = 0f; // Stop movement
            Debug.Log("Enemy frozen!");
        }
        else
        {
            speed = originalSpeed; // Restore original movement speed
            Debug.Log("Enemy unfrozen!");
        }
    }

    public IEnumerator ApplyFreezeEffect(EnemyCtrl enemy)
    {
        Debug.Log("Freezing enemy...");
        enemy.Freeze(true); // Freeze the enemy
        Debug.Log($"Waiting for {freezeDuration} seconds...");
        yield return new WaitForSeconds(freezeDuration); // Wait for freeze duration

        Debug.Log("Unfreezing enemy...");
        enemy.Freeze(false); // Unfreeze the enemy
        Debug.Log("Enemy unfrozen!");
    }

        public void ApplyBurnEffect(float totalDamage, int burnTicks, float tickInterval)
    {
        StartCoroutine(TakeBurnDamage(totalDamage, burnTicks, tickInterval));
    }

    private IEnumerator TakeBurnDamage(float totalDamage, int burnTicks, float tickInterval)
    {
        Debug.Log($"Enemy burning for {totalDamage} total damage over {burnTicks} ticks.");
        float damagePerTick = totalDamage / burnTicks;

        for (int i = 0; i < burnTicks; i++)
        {
            if (currentHealth <= 0)
            {
                Debug.Log("Enemy already dead during burn effect. Stopping burn.");
                yield break;
            }

            TakeDamage(damagePerTick);
            Debug.Log($"Burn tick {i + 1}/{burnTicks}: {damagePerTick} damage applied. Current health: {currentHealth}");

            yield return new WaitForSeconds(tickInterval); // Wait for the next tick
        }

        Debug.Log("Burn effect ended.");
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy Health: {currentHealth}");

        if (healthBar != null)
        {
            healthBar.value = currentHealth; // Update health bar
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy Died!");
        Destroy(gameObject); // Destroy the enemy
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Deal damage to the player
            PlayerCtrl playerCtrl = collision.GetComponent<PlayerCtrl>();
            if (playerCtrl != null)
            {
                playerCtrl.TakeDamage(attackDamage);
                Debug.Log($"Enemy attacked player! Damage: {attackDamage}");
            }

            // Destroy the enemy upon collision
            Destroy(gameObject);
        }
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
