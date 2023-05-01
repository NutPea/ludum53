using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICountDown : MonoBehaviour
{
    public CarTimeHandler carTimeHandler;
    public TextMeshProUGUI countDownText;
    public GameObject moneyUI;
    public GameObject timeUI;
    void Awake()
    {
        carTimeHandler ??= GameObject.FindGameObjectWithTag("Player").GetComponent<CarTimeHandler>();
        carTimeHandler.OnCountDownUpdate.AddListener(UpdateCountDown);
        countDownText.gameObject.SetActive(true);
        moneyUI.gameObject.SetActive(false);
        timeUI.gameObject.SetActive(false);
    }

    private void UpdateCountDown(int time)
    {
        countDownText.text = time.ToString();
       if(time== 0)
       {
            countDownText.text = "GO!";
            Invoke(nameof(RemoveTime), 0.5f);
       }
    }

    private void RemoveTime()
    {
        countDownText.gameObject.SetActive(false);
        moneyUI.gameObject.SetActive(true);
        timeUI.gameObject.SetActive(true);
    }


}
