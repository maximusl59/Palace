using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SeatsAssignedEvent
{
    public static event Action OnSeatsAssigned;

    public static void OnOnSeatsAssigned() {
        var handler = OnSeatsAssigned;
        if (handler != null) OnSeatsAssigned();
    }
}
