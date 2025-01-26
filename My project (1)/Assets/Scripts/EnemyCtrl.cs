using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCtrl : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public float maxHealth = 50f;
    public float currentHealth;
    public float originalSpeed = 2f;

    public float freezeDuration = 2f;

    private Rigidbody2D rb;
    private PlayerAwarenessController playerAwarenessController;
    private Vector2 targetDirection;

    public float attackDamage = 50f;

    public Slider healthBar;
    public Vector3 healthBarOffset = new Vector3(0, 1.5f, 0);
    private Quaternion fixedRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAwarenessController = GetComponent<PlayerAwarenessController>();
        currentHealth = maxHealth;
        originalSpeed = speed;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
            fixedRotation = healthBar.transform.rotation;
        }
    }

    private void Update()
    {
        UpdateTargetDirection();
        RotateTowardsTarget();
        UpdateHealthBarPosition();
    }

    private void FixedUpdate()
    {
        MoveTowardsTarget();
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

    private void MoveTowardsTarget()
    {
        if (targetDirection == Vector2.zero)
        {
            rb.velocity = Vector2.zero;
        }
        else
        {
            rb.velocity = targetDirection.normalized * speed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Deal damage to the player
            PlayerCtrl playerCtrl = collision.gameObject.GetComponent<PlayerCtrl>();
            if (playerCtrl != null)
            {
                playerCtrl.TakeDamage(attackDamage);
                Debug.Log($"Enemy attacked player! Damage: {attackDamage}");
            }

            // Destroy the enemy upon collision with the player
            Destroy(gameObject);
        }

        // Handle collision with walls
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Enemy collided with a wall.");
            // You can add additional behavior here if needed
        }
    }

    public void Freeze(bool isFrozen)
    {
        if (isFrozen)
        {
            speed = 0f;
            Debug.Log("Enemy frozen!");
        }
        else
        {
            speed = originalSpeed;
            Debug.Log("Enemy unfrozen!");
        }
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

            yield return new WaitForSeconds(tickInterval);
        }

        Debug.Log("Burn effect ended.");
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy Health: {currentHealth}");

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy Died!");
        Destroy(gameObject);
    }

    private void UpdateHealthBarPosition()
    {
        if (healthBar != null)
        {
            healthBar.transform.position = transform.position + healthBarOffset;
            healthBar.transform.rotation = fixedRotation;
        }
    }
}
