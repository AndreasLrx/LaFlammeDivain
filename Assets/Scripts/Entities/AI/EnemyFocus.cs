using System.Collections;
using UnityEngine;
using NetMQ.Sockets;
using NetMQ;


public class EnemyFocus : MonoBehaviour
{
    private AICompanion _aiCompanion;
    protected AICompanion aiCompanion { get { return _aiCompanion; } }
    protected RequestSocket client { get { return aiCompanion.client; } }

    protected virtual void Awake()
    {
        _aiCompanion = GetComponentInParent<AICompanion>();
    }

    private void Update()
    {
        RequestDecision();
    }

    private void RequestDecision()
    {
        client.SendFrame("Hello");

        string message = null;
        bool gotMessage = false;
        while (true)
        {
            gotMessage = client.TryReceiveFrameString(out message); // this returns true if it's successful
            if (gotMessage)
                break;
        }

        if (gotMessage)
            Debug.Log("Received " + message);
    }
}
