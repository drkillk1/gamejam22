using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float baseDamage = 10f; // Base damage dealt by the bullet
    private bool isCharged = false; // Tracks whether the bullet has been fired

    public void SetIsCharged(bool value)
    {
        isCharged = value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Damage regular enemies
        if (isCharged && collision.GetComponent<EnemyCtrl>())
        {
            float bulletSize = transform.localScale.x; // Use the scale of the bullet to calculate damage
            float damage = baseDamage * bulletSize; // Larger bullets deal more damage
            Debug.Log($"Bullet hit melee enemy! Damage: {damage}");

            EnemyCtrl enemyCtrl = collision.GetComponent<EnemyCtrl>();
            if (enemyCtrl != null)
            {
                enemyCtrl.TakeDamage(damage); // Deal damage to the melee enemy
            }

            Destroy(gameObject); // Destroy the bullet
        }

        // Damage shooting enemies
        if (isCharged && collision.GetComponent<ShootingEnemyCtrl>())
        {
            float bulletSize = transform.localScale.x; // Use the scale of the bullet to calculate damage
            float damage = baseDamage * bulletSize; // Larger bullets deal more damage
            Debug.Log($"Bullet hit shooting enemy! Damage: {damage}");

            ShootingEnemyCtrl shootingEnemy = collision.GetComponent<ShootingEnemyCtrl>();
            if (shootingEnemy != null)
            {
               shootingEnemy.TakeDamage(damage); // Deal damage to the shooting enemy
            }

            Destroy(gameObject); // Destroy the bullet
        }

        if (isCharged && collision.GetComponent<SpinningShootingEnemyCtrl>())
        {
            float bulletSize = transform.localScale.x; // Use the scale of the bullet to calculate damage
            float damage = baseDamage * bulletSize; // Larger bullets deal more damage
            Debug.Log($"Bullet hit spinning shooting enemy! Damage: {damage}");

            SpinningShootingEnemyCtrl spinningShootingEnemy = collision.GetComponent<SpinningShootingEnemyCtrl>();
            if (spinningShootingEnemy != null)
            {
                spinningShootingEnemy.TakeDamage(damage); // Deal damage to the shooting enemy
            }

            Destroy(gameObject); // Destroy the bullet
        }
        
        // Damage the player during charging
        if (collision.CompareTag("Player"))
        {
            float bulletSize = transform.localScale.x; // Use the scale of the bullet to calculate damage
            float damage = baseDamage * bulletSize;
            Debug.Log($"Bullet hit player! Damage: {damage}");

            PlayerCtrl playerCtrl = collision.GetComponent<PlayerCtrl>();
            if (playerCtrl != null)
            {
                playerCtrl.TakeDamage(damage); // Deal damage to the player
            }

            Destroy(gameObject); // Destroy the bullet
        }
    }
}
