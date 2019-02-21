using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{

    Vector3 myPos;
    public bool isActivated;
    public GameObject Player;
    public PlayerController playerController;

    void Start()
    {
        myPos = transform.position;
        Player = GameObject.FindGameObjectWithTag("Player");
        playerController = Player.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Player" && !isActivated)
        {
            isActivated = true;
            playerController.spawnPoint = myPos;
        }
    }

}
