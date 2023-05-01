using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiTimeHandler : MonoBehaviour
{
    public CarTimeHandler carTimeHandler;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI addTimeText;
    void Start()
    {

        carTimeHandler ??= GameObject.FindGameObjectWithTag("Player").GetComponent<CarTimeHandler>();
        carTimeHandler.OnTimeUpdate.AddListener(UpdateTimeDisplay);
        carTimeHandler.OnAddTimeUpdate.AddListener(UpdateAddTimeDisplay);
        addTimeText.gameObject.SetActive(false);
    }

    private void UpdateAddTimeDisplay(string amount)
    {
        addTimeText.gameObject.SetActive(true);
        addTimeText.text = amount;
        Invoke(nameof(RemoveAddTime), 0.5f);
    }

    private void UpdateTimeDisplay(string currentTime)
    {
        timerText.text = currentTime;
    }

    private void RemoveAddTime()
    {
        addTimeText.gameObject.SetActive(false);
    }
}
