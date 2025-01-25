using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerAwarenessController : MonoBehaviour
{
    public bool AwareOfPlayer { get; private set; }
    public UnityEngine.Vector2 DirectionToPlayer { get; private set; }

    public float playerAwarenessDistance;
    private Transform player;

    // Start is called before the first frame update
    private void Awake()
    {
        player = FindObjectOfType<PlayerCtrl>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Vector2 enemyToPlayerVector = player.position - transform.position;
        DirectionToPlayer = enemyToPlayerVector.normalized;

        if(enemyToPlayerVector.magnitude <= playerAwarenessDistance)
        {
            AwareOfPlayer = true;
        } 
        else{
            AwareOfPlayer = false;
        }
    }
}
