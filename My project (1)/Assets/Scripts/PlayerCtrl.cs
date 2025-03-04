using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    public enum AbilityType { Normal, Fire, Ice }
    public AbilityType currentAbility = AbilityType.Normal;

    public float maxHealth = 100f;
    public float currentHealth;

    public bool hasKey;

    Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 smoothedMovementInput;
    private Vector2 movementInputSmoothVelocity;
    public Camera cam;
    [SerializeField]
    private float rotationSpeed;
    public float speed;

    public Slider playerHealthBar; // Player health bar slider
    public Text abilityDisplayText; // UI Text element to display the current ability

    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth; // Initialize player health

        if (playerHealthBar != null)
        {
            playerHealthBar.maxValue = maxHealth;
            playerHealthBar.value = currentHealth;
        }

        UpdateAbilityUI();
    }

    void Update()
    {
        SetPlayerVelocity();
        RotateInDirectionOfCursor();
        HandleAbilitySwitching();
        Debug.Log($"Player position now: {transform.position}");
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

        Debug.Log($"Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        Debug.Log($"Mouse World Position: {mouseWorldPosition}");
        Debug.Log($"Player Position: {transform.position}");
        Debug.Log($"Target Direction: {targetDirection}");

        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, targetDirection);
        var rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        rb.MoveRotation(rotation);
    }

    public void Heal(float amount)
    {
        currentHealth += amount; // Increase health
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Prevent overhealing
        Debug.Log($"Player healed! Current health: {currentHealth}");

        // Update the health bar UI
        if (playerHealthBar != null)
        {
            playerHealthBar.value = currentHealth;
        }
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

    private void HandleAbilitySwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentAbility = AbilityType.Normal;
            Debug.Log("Switched to Normal Ability");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentAbility = AbilityType.Fire;
            Debug.Log("Switched to Fire Ability");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentAbility = AbilityType.Ice;
            Debug.Log("Switched to Ice Ability");
        }

        UpdateAbilityUI();
    }

    private void UpdateAbilityUI()
    {
        if (abilityDisplayText != null)
        {
            abilityDisplayText.text = $"Ability: {currentAbility}";
        }
    }
}
