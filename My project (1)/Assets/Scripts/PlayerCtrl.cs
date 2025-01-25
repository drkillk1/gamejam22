using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCtrl : MonoBehaviour
{
    
    Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 smoothedMovementInput;
    private Vector2 movementInputSmoothVelocity;
    public Camera cam;
    [SerializeField]
    private float rotationSpeed;
    public float speed;

    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
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
    
    
    
}
