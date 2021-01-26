using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGrab : MonoBehaviour
{





    private GameObject player;

    void Awake(){

        player = GameObject.Find("Player");
    }

    private void OnTriggerEnter(Collider collider)
    {

        if (collider.gameObject.tag == "Player")
        {
            player.GetComponent<PlayerMovement>().isGrabbed = true;
            player.GetComponent<PlayerMovement>().whoGrabbed = this.gameObject;
        }

    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            player.GetComponent<PlayerMovement>().isGrabbed = false;
        }

    }
}
