using System;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    public static event Action OnPlayerPunch;

    public void InvokePunchEvent() => OnPlayerPunch?.Invoke();
}