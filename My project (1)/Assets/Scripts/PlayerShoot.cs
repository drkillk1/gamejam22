using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;

    public Transform gunOffset;

    public float timeBetweenShots;

    public float bulletSpeed;

    private bool fireContinously;
    private bool fireSingle;
    private float lastFireTime;
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    void Update()
    {
        if(fireContinously || fireSingle)
        {
            float timeSinceLastFire = Time.time - lastFireTime;

            if (timeSinceLastFire >= timeBetweenShots)
            {
                FireBullet();

                lastFireTime = Time.time;
                fireSingle = false;
            }
            
        }
    }

    private void FireBullet()
    {
        GameObject bubble = Instantiate(bulletPrefab, transform.position, transform.rotation);
        Rigidbody2D rb = bubble.GetComponent<Rigidbody2D>();

        rb.velocity = bulletSpeed * transform.up;
    }

    private void OnFire(InputValue inputValue)
    {
        fireContinously = inputValue.isPressed;

        if (inputValue.isPressed)
        {
            fireSingle = true;
        }
    }
}
