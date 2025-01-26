using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public enum BubbleType { Normal, Fire, Ice} // Different types of bubbles
    public BubbleType bubbleType = BubbleType.Normal;

    public float baseDamage = 10f; // Base damage dealt by the bubble
    private bool isCharged = false; // Tracks whether the bubble has been fired
    public float damageMultiplier = 2f; // Multiplier to scale damage based on bubble size

    // Fire bubble-specific properties
    public float burnDamage = 5f; // Damage per second for burning effect
    public float burnDuration = 3f; // Duration of the burning effect

    // Ice bubble-specific properties
    public float freezeDuration = 2f; // Duration of the freezing effect

    [SerializeField] private float effectSizeThreshold = 1.5f; // Minimum bubble size for special effects


    public void SetIsCharged(bool value)
    {
        isCharged = value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isCharged) return;

        // Apply effects to enemies
        if (collision.GetComponent<EnemyCtrl>())
        {
            ApplyEffect(collision.GetComponent<EnemyCtrl>());
        }
        else if (collision.GetComponent<ShootingEnemyCtrl>())
        {
            ApplyEffect(collision.GetComponent<ShootingEnemyCtrl>());
        }
        else if (collision.GetComponent<SpinningShootingEnemyCtrl>())
        {
            ApplyEffect(collision.GetComponent<SpinningShootingEnemyCtrl>());
        }

        // Damage the player during charging
        if (collision.CompareTag("Player"))
        {
            float bulletSize = transform.localScale.x; // Use the scale of the bubble to calculate damage
            float damage = baseDamage * bulletSize;
            Debug.Log($"Bullet hit player! Damage: {damage}");

            PlayerCtrl playerCtrl = collision.GetComponent<PlayerCtrl>();
            if (playerCtrl != null)
            {
                playerCtrl.TakeDamage(damage); // Deal damage to the player
            }
        }

        Destroy(gameObject); // Destroy the bubble
    }

    private void ApplyEffect(MonoBehaviour enemy)
    {
        float bulletSize = transform.localScale.x;
        float damage = baseDamage + (bulletSize * damageMultiplier);
        Debug.Log($"Bubble hit enemy! Type: {bubbleType}, Damage: {damage}");

        if (bulletSize < effectSizeThreshold)
        {
            Debug.Log($"Bubble too small to activate effects. Size: {bulletSize}, Threshold: {effectSizeThreshold}");
            ApplyNormalDamage(enemy, damage); // Apply normal damage only
            Destroy(gameObject); // Destroy the bubble
            return;
        }
        switch (bubbleType)
        {
            case BubbleType.Fire:
                Debug.Log("Applying Fire effect!");
                int burnTicks = (int)burnDuration; // Number of ticks
                float tickInterval = 1f; // Interval between ticks
                if (enemy is EnemyCtrl regularEnemy)
                {
                    regularEnemy.ApplyBurnEffect(burnDamage, burnTicks, tickInterval);
                }
                else if (enemy is ShootingEnemyCtrl shootingEnemy)
                {
                    shootingEnemy.ApplyBurnEffect(burnDamage, burnTicks, tickInterval);
                }
                else if (enemy is SpinningShootingEnemyCtrl spinningEnemy)
                {
                    spinningEnemy.ApplyBurnEffect(burnDamage, burnTicks, tickInterval);
                }
                break;

            case BubbleType.Ice:
                Debug.Log("Applying Ice effect!");
                if (enemy is EnemyCtrl regularEnemy1)
                {
                    regularEnemy1.TakeDamage(damage); // Apply immediate damage
                    regularEnemy1.StartCoroutine(ApplyFreezeEffect(regularEnemy1)); // Freeze the enemy
                }
                else if (enemy is ShootingEnemyCtrl shootingEnemy)
                {
                    shootingEnemy.TakeDamage(damage); // Apply immediate damage
                    shootingEnemy.StartCoroutine(ApplyFreezeEffect(shootingEnemy));
                }
                else if (enemy is SpinningShootingEnemyCtrl spinningEnemy)
                {
                    spinningEnemy.TakeDamage(damage); // Apply immediate damage
                    spinningEnemy.StartCoroutine(ApplyFreezeEffect(spinningEnemy));
                }
                break;

            case BubbleType.Normal:
                Debug.Log("Applying Normal damage!");
                ApplyNormalDamage(enemy, damage); // Direct damage application for normal bubbles
                break;

            default:
                Debug.LogWarning("Unknown bubble type!");
                break;
        }

        Destroy(gameObject); // Destroy the bubble after hitting
    }





    private void ApplyNormalDamage(MonoBehaviour enemy, float damage)
    {
        if (enemy is EnemyCtrl regularEnemy)
        {
            regularEnemy.TakeDamage(damage);
        }
        else if (enemy is ShootingEnemyCtrl shootingEnemy)
        {
            shootingEnemy.TakeDamage(damage);
        }
        else if (enemy is SpinningShootingEnemyCtrl spinningEnemy)
        {
            spinningEnemy.TakeDamage(damage);
        }
    }

    private IEnumerator ApplyBurnEffect(MonoBehaviour enemy, float totalDamage)
    {
        Debug.Log("Starting burn effect...");

        int burnTicks = 5; // Number of damage ticks
        float damagePerTick = totalDamage / burnTicks; // Divide total damage into ticks
        float tickInterval = 1f; // Time interval between ticks

        for (int i = 0; i < burnTicks; i++)
        {
            if (enemy == null)
            {
                Debug.LogWarning("Enemy is null during burn effect. Stopping burn.");
                yield break; // Stop if the enemy is destroyed
            }

            if (enemy is EnemyCtrl regularEnemy)
            {
                regularEnemy.TakeDamage(damagePerTick);
                Debug.Log($"Burn tick {i + 1}/{burnTicks}: {damagePerTick} damage applied. Enemy health: {regularEnemy.currentHealth}");
            }
            else if (enemy is ShootingEnemyCtrl shootingEnemy)
            {
                shootingEnemy.TakeDamage(damagePerTick);
                Debug.Log($"Burn tick {i + 1}/{burnTicks}: {damagePerTick} damage applied. Enemy health: {shootingEnemy.currentHealth}");
            }
            else if (enemy is SpinningShootingEnemyCtrl spinningEnemy)
            {
                spinningEnemy.TakeDamage(damagePerTick);
                Debug.Log($"Burn tick {i + 1}/{burnTicks}: {damagePerTick} damage applied. Enemy health: {spinningEnemy.currentHealth}");
            }

            yield return new WaitForSeconds(tickInterval); // Wait before the next tick
        }

        Debug.Log("Burn effect ended.");
    }



    private IEnumerator ApplyFreezeEffect(MonoBehaviour enemy)
    {
        Debug.Log("Starting ApplyFreezeEffect...");

        if (enemy is EnemyCtrl regularEnemy)
        {
            Debug.Log("Freezing regular enemy!");
            regularEnemy.Freeze(true); // Freeze the enemy
            Debug.Log($"Waiting for {freezeDuration} seconds...");
            yield return new WaitForSeconds(freezeDuration); // Wait for freeze duration
            Debug.Log("Time's up! Attempting to unfreeze...");
            regularEnemy.Freeze(false); // Unfreeze the enemy
            Debug.Log("Regular enemy unfrozen!");
        }
        else if (enemy is SpinningShootingEnemyCtrl spinningEnemy)
        {
            Debug.Log("Freezing spinning enemy!");
            spinningEnemy.Freeze(true);
            Debug.Log($"Waiting for {freezeDuration} seconds...");
            yield return new WaitForSeconds(freezeDuration);
            Debug.Log("Time's up! Attempting to unfreeze...");
            spinningEnemy.Freeze(false);
            Debug.Log("Spinning enemy unfrozen!");
        }
        else if (enemy is ShootingEnemyCtrl shootingEnemy)
        {
            Debug.Log("Freezing shooting enemy!");
            shootingEnemy.Freeze(true);
            Debug.Log($"Waiting for {freezeDuration} seconds...");
            yield return new WaitForSeconds(freezeDuration);
            Debug.Log("Time's up! Attempting to unfreeze...");
            shootingEnemy.Freeze(false);
            Debug.Log("Shooting enemy unfrozen!");
        }
    }


}
