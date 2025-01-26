using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachUIToPlayer : MonoBehaviour
{
    public Canvas chargeBarCanvas; // Assign the Canvas in the Inspector
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Position offset from player

    void Start()
    {
        if (chargeBarCanvas != null)
        {
            // Set the Canvas to World Space
            chargeBarCanvas.renderMode = RenderMode.WorldSpace;
            chargeBarCanvas.transform.SetParent(transform, false);
            chargeBarCanvas.transform.localPosition = offset;
            chargeBarCanvas.transform.localScale = Vector3.one * 0.01f; // Adjust scale for visibility
        }
    }
}

