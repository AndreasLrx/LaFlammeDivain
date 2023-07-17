using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AsyncEventsProcessor : MonoBehaviour
{
    public delegate IEnumerator AsyncEvent();

    public IEnumerator StartAsyncEvents(AsyncEvent events)
    {
        if (events == null)
            yield break;

        List<IEnumerator> coroutines = new List<IEnumerator>();

        Delegate[] delegates = events.GetInvocationList();
        foreach (AsyncEvent del in delegates.Cast<AsyncEvent>())
            yield return StartCoroutine(del());
    }
}
