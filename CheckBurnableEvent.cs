using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CheckBurnableEvent
{
    public static event Action OnCheckBurnable;

    public static void OnOnCheckBurnable() {
        var handler = OnCheckBurnable;
        if (handler != null) OnCheckBurnable();
    }
}
