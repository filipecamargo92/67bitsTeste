using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyValue;

    int moneyAmt;

    public static event Action OnPlayerSell;

    public void IncreaseMoneyValue(int amt)
    {
        moneyAmt += amt;
        moneyValue.text = moneyAmt.ToString();
        OnPlayerSell?.Invoke();
    }
}
