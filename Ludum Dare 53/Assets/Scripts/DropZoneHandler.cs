using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZoneHandler : MonoBehaviour
{

    public Transform dropPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CarPassengerPickUpHandler carPassengerPickUpHandler = other.gameObject.GetComponent<CarPassengerPickUpHandler>();
            if (carPassengerPickUpHandler.HasPassenger)
            {
                carPassengerPickUpHandler.DropPassenger(this);
                Destroy(gameObject);
            }
        }
    }

    
}
