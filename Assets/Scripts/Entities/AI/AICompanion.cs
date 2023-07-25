using System.Collections;
using UnityEngine;


public class AICompanion : Singleton<AICompanion>
{
    public RoomGenerator roomGenerator;
    public Transform target;
    public Behavior movement;

    private Player _player;
    public Player player { get { return _player; } }


    private void Start()
    {
        _player = GetComponent<Player>();
        movement = GetComponentInChildren<Behavior>();
    }
}
