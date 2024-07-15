using System;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] LayerMask playerLayer;

    public static event Action OnPlayerSell;

    private void OnTriggerEnter(Collider other)
    {
        if ((playerLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            OnPlayerSell?.Invoke();
        }
    }
}
