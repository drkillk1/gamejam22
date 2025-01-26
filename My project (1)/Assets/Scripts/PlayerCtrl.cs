using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 smoothedMovementInput;
    private Vector2 movementInputSmoothVelocity;
    public Camera cam;
    [SerializeField]
    private float rotationSpeed;
    public float speed;

    public Slider playerHealthBar; // Player health bar slider

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth; // Initialize player health

        if (playerHealthBar != null)
        {
            playerHealthBar.maxValue = maxHealth;
            playerHealthBar.value = currentHealth;
        }
    }

    void Update()
    {
        SetPlayerVelocity();
        RotateInDirectionOfCursor();
    }

    private void SetPlayerVelocity()
    {
        smoothedMovementInput = Vector2.SmoothDamp(
            smoothedMovementInput,
            movementInput,
            ref movementInputSmoothVelocity,
            0.1f);

        rb.velocity = smoothedMovementInput * speed;
    }

    private void RotateInDirectionOfCursor()
    {
        Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 targetDirection = mouseWorldPosition - transform.position;
        targetDirection.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, targetDirection);
        var rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        rb.MoveRotation(rotation);
    }

    private void OnMove(InputValue inputValue)
    {
        movementInput = inputValue.Get<Vector2>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Player Health: {currentHealth}");

        if (playerHealthBar != null)
        {
            playerHealthBar.value = currentHealth; // Update health bar
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        // Implement respawn or game over logic here
    }
}
