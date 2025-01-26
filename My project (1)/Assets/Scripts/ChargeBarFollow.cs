using UnityEngine;

public class ChargeBarFollow : MonoBehaviour
{
    public Transform player; // Assign the player's Transform in the Inspector
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Offset from the player's position

    private Quaternion fixedRotation;

    void Start()
    {
        // Save the initial rotation of the bar (so it doesn’t rotate later)
        fixedRotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (player != null)
        {
            // Follow the player's position while maintaining the fixed rotation
            transform.position = player.position + offset;
            transform.rotation = fixedRotation;
        }
    }
}
