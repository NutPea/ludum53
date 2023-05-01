using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarPassengerPickUpHandler : MonoBehaviour
{

    public bool HasPassenger => hasPassenger;
    private bool hasPassenger;

    public Transform carPassengerPosition;
    [HideInInspector]public CustomerHandler currentCustomerHandler;

    [HideInInspector] public UnityEvent OnPickUpCustomer = new();
    [HideInInspector] public UnityEvent OnDropCustomer = new();
    public UnityEvent<bool> OnHitEnemy = new();

    private void Start()
    {
        OnHitEnemy.AddListener(OnHitOtherMutant);
    }

    private void OnHitOtherMutant(bool isSameMutant)
    {
        if (isSameMutant)
        {
            currentCustomerHandler.HasKilledWrongMutant();
        }
        else
        {
            currentCustomerHandler.HasKilledRightMutant();
        }
    }

    public void AddPassenger(CustomerHandler customerHandler)
    {
        currentCustomerHandler = customerHandler;
        customerHandler.transform.parent = carPassengerPosition;
        customerHandler.transform.forward = carPassengerPosition.transform.forward;
        customerHandler.transform.localPosition = Vector3.zero;
        hasPassenger = true;
    }

    public void DropPassenger(DropZoneHandler dropZoneHandler)
    {
        currentCustomerHandler.transform.parent = null;
        currentCustomerHandler.transform.position = dropZoneHandler.dropPosition.position;
        currentCustomerHandler.transform.forward = dropZoneHandler.dropPosition.forward;
        currentCustomerHandler.DropPassenger();
        hasPassenger = false;
        currentCustomerHandler = null;
        OnDropCustomer.Invoke();
    }
    
}
