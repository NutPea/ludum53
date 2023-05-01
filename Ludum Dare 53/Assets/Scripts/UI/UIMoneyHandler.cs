using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIMoneyHandler : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI addMoneyText;
    public TextMeshProUGUI removeMoneyText;

    public CarMoneyHandler carMoneyHandler;
    public float goAwayTimer = 2f;
    void Awake()
    {
        RemoveText();
        carMoneyHandler ??= GameObject.FindGameObjectWithTag("Player").GetComponent<CarMoneyHandler>();
        carMoneyHandler.OnMoneyUpdate.AddListener((amount) =>
        {
            moneyText.text = amount.ToString();
        });

        carMoneyHandler.OnAddMoneyUpdate.AddListener((amount) =>
        {
            addMoneyText.gameObject.SetActive(true);
            addMoneyText.text ="+"+ amount.ToString();
            Invoke(nameof(RemoveText), goAwayTimer);
        });

        carMoneyHandler.OnRemoveMoneyUpdate.AddListener((amount) =>
        {
            removeMoneyText.gameObject.SetActive(true);
            removeMoneyText.text = "-" + amount.ToString();
            Invoke(nameof(RemoveText), goAwayTimer);
        });
    }

    public void RemoveText()
    {
        addMoneyText.gameObject.SetActive(false);
        removeMoneyText.gameObject.SetActive(false);
    }


}
