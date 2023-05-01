using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarMoneyHandler : MonoBehaviour
{
    public int currentPointAmount = 0;
    private CarPassengerPickUpHandler carPassengerPickUpHandler;


    [HideInInspector] public UnityEvent<int> OnMoneyUpdate = new();
    [HideInInspector] public UnityEvent<int> OnAddMoneyUpdate = new();
    [HideInInspector] public UnityEvent<int> OnRemoveMoneyUpdate = new();

    public int pickUpAddAmount= 20;
    public int deliverAddAmount = 100;
    public int killRightMutant = 50;
    public int killWrongMutant = 100;

    private const float POINT_UPDATE_TIME = 0.5f;
    private bool canChangePoints;
    private void Start()
    {
        carPassengerPickUpHandler = GetComponent<CarPassengerPickUpHandler>();
        carPassengerPickUpHandler.OnPickUpCustomer.AddListener(() => AddPoints(pickUpAddAmount));
        carPassengerPickUpHandler.OnDropCustomer.AddListener(() => AddPoints(deliverAddAmount));
        carPassengerPickUpHandler.OnHitEnemy.AddListener((sameMutant) =>
        {
            if (sameMutant)
            {
                RemovePoints(killWrongMutant);
            }
            else
            {
                AddPoints(killRightMutant);
            }
        });
        OnMoneyUpdate.Invoke(currentPointAmount);
        canChangePoints = true;
    }

    public void AddPoints(int amount)
    {
        if (!canChangePoints){
            return;
        }
        currentPointAmount += amount;
        OnMoneyUpdate.Invoke(currentPointAmount);
        if(amount > 0)
        {
            OnAddMoneyUpdate.Invoke(amount);
        }
        Invoke(nameof(ResetCooldown), POINT_UPDATE_TIME);
    }

    public void RemovePoints(int amount)
    {
        if (!canChangePoints){
            return;
        }
        currentPointAmount -= amount;
        if(currentPointAmount< 0)
        {
            currentPointAmount = 0;
        }
        OnMoneyUpdate.Invoke(currentPointAmount);
        if (amount > 0)
        {
            OnRemoveMoneyUpdate.Invoke(amount);
        }
        Invoke(nameof(ResetCooldown), POINT_UPDATE_TIME);
    }

    public void ResetCooldown()
    {
        canChangePoints = true;
    }
}
