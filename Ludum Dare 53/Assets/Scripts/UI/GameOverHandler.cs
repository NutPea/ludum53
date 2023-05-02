using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    public CarController carController;
    public CarTimeHandler timeHandler;
    public CarMoneyHandler moneyHandler;

    public GameObject GameOverObject;
    public List<GameObject> removeUI;
    public TextMeshProUGUI scoreMoneyText;

    void Start()
    {
        carController ??= GameObject.FindGameObjectWithTag("Player").GetComponent<CarController>();
        timeHandler ??= GameObject.FindGameObjectWithTag("Player").GetComponent<CarTimeHandler>();
        moneyHandler ??= GameObject.FindGameObjectWithTag("Player").GetComponent<CarMoneyHandler>();
        timeHandler.OnTimerHasFinished.AddListener(TimeIsOver);
    }

    private void TimeIsOver()
    {
        foreach(GameObject ui in removeUI)
        {
            ui.SetActive(false);
        }
        GameOverObject.SetActive(true);
        scoreMoneyText.text = "Score:" + moneyHandler.currentPointAmount;
        carController.stopInput = true;

    }


}
