using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarTimeHandler : MonoBehaviour
{

    
    public float countDownTimer;

    public float startTimer = 10f;
    public float currentTimer = 0;

    public float amountToAddPerPassenger = 10f;

    [HideInInspector]public UnityEvent OnCountDownFinished = new();

    [HideInInspector]public UnityEvent<string> OnTimeUpdate = new();
    [HideInInspector] public UnityEvent<string> OnAddTimeUpdate = new();
    [HideInInspector]public UnityEvent OnTimerHasFinished = new();

    private bool hasBeenCountDown = true;
    private CarPassengerPickUpHandler pickUpHandler;

    void Start()
    {
        currentTimer = startTimer;
        pickUpHandler = GetComponent<CarPassengerPickUpHandler>();
        pickUpHandler.OnDropCustomer.AddListener(AddTime);
        pickUpHandler.OnPickUpCustomer.AddListener(AddTime);
    }

    // Update is called once per frame
    void Update()
    {
        //ToDo:Add CountDown

        if (hasBeenCountDown)
        {
            if(currentTimer < 0) {
                OnTimeUpdate.Invoke("0.00");
                OnTimerHasFinished.Invoke();
                return;
            }
            else{
                currentTimer -= Time.deltaTime;
                OnTimeUpdate.Invoke(currentTimer.ToString("#,00"));
            }
        }
    }

    public void AddTime()
    {
        currentTimer += amountToAddPerPassenger;
        OnTimeUpdate.Invoke(currentTimer.ToString("#,00"));
        OnAddTimeUpdate.Invoke(amountToAddPerPassenger.ToString("#,00"));
    }
}
