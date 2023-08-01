using System.Collections;
using NetMQ.Sockets;
using NetMQ;
using AsyncIO;
using UnityEngine;


public class AICompanion : Singleton<AICompanion>
{
    public RoomGenerator roomGenerator;
    public Transform target;
    public Behavior movement;
    public Behavior enemyFocus;

    private Player _player;
    public Player player { get { return _player; } }

    private RequestSocket _client = new();
    public RequestSocket client { get { return _client; } }


    private void Start()
    {
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet
        _player = GetComponent<Player>();
        client.Connect("tcp://localhost:5555");
    }

    private void OnDestroy()
    {
        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }
}
