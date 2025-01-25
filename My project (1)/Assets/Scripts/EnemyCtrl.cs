using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyCtrl : MonoBehaviour
{
    public float speed;

    public float rotationSpeed;

    private Rigidbody2D rb;
    private PlayerAwarenessController playerAwarenessController;
    private Vector2 targetDirection;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAwarenessController = GetComponent<PlayerAwarenessController>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTargetDirection();
        RotateTowardsTarget();
        SetVelocity();

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
}
