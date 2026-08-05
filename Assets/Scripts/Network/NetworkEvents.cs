using System;
using UnityEngine;

public class NetworkEvents
{
    public static event Action CreateAccountSuccessed;

    public static void RaiseCreateAccountSuccessed() => CreateAccountSuccessed?.Invoke();
}
