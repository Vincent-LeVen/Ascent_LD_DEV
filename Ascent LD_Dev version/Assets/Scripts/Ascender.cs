using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ascender : MonoBehaviour
{
    private GameObject Player;
    private Rigidbody playerRigidbody;
    private PlayerController playerController;
    public float AscenderForce = 12f;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerRigidbody = Player.GetComponent<Rigidbody>();
        playerController = Player.GetComponent<PlayerController>();
    }

    private void OnTriggerStay(Collider coll)
    {
        if (playerController.isAlenvers)
        {
            if (coll.gameObject.tag == "Player")
            {
                playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, -AscenderForce , playerRigidbody.velocity.z);
            }
        }
        else
        {
            if (coll.gameObject.tag == "Player")
            {
                playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, AscenderForce, playerRigidbody.velocity.z);
            }
        }


        
    }
}
