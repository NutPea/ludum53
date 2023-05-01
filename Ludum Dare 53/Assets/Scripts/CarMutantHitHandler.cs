using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarMutantHitHandler : MonoBehaviour
{

    CarPassengerPickUpHandler carPassengerPickUpHandler;
    private void Start()
    {
        carPassengerPickUpHandler = GetComponent<CarPassengerPickUpHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            MutantHandler mutantHandler = other.gameObject.GetComponent<MutantHandler>();
            if (carPassengerPickUpHandler.HasPassenger)
            {
                carPassengerPickUpHandler.OnHitEnemy.Invoke(carPassengerPickUpHandler.currentCustomerHandler.currentMutantTyp == mutantHandler.currentMutantTyp);
            }
            mutantHandler.Kill(transform);
        }
    }
}
