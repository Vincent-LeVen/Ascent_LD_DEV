using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachPlayer : MonoBehaviour
{
    public GameObject Player;
    public GameObject PlayerHolder;
    private PlayerController playerController;
    public bool isCrusher = false;
    private bool isAttaching = false;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerController = Player.GetComponent<PlayerController>();
        PlayerHolder = GameObject.FindGameObjectWithTag("PlayerHolder");
        if (this.gameObject.tag == "Crusher")
        {
            isCrusher = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Player)
        {
            PlayerHolder.transform.parent = transform;
            if (playerController.isAttachedToOne == false && isAttaching == false)
            {
                playerController.isAttachedToOne = true;
                if (isCrusher == true)
                {
                    playerController.isAttachedToCrusher = true;
                }
            }
            else if (playerController.isAttachedToOne == true && isAttaching == false)
            {
                if (isCrusher == true || playerController.isAttachedToCrusher == true)
                {
                    playerController.callDeath = true;
                }
            }
            isAttaching = true;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == Player)
        {
            PlayerHolder.transform.parent = null;
            playerController.isAttachedToOne = false;
            isAttaching = false;
        }
    }

}
