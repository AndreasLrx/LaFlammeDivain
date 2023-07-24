using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<Player>
{
    private Player _player;
    public Player player { get { return _player; } }
    public static bool isPaused = false;

    protected override void Awake()
    {
        base.Awake();
        _player = GetComponent<Player>();
    }

    void Update()
    {
        if (isPaused)
            return;
        player.aimedDirection = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;
        player.moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetButtonDown("Attack"))
            player.Attack();
        if (Input.GetButtonDown("SecondaryAttack"))
            player.SecondaryAttack();
        if (Input.GetButtonDown("ToggleWeaponMode"))
            player.ToggleWeaponMode();
        if (Input.GetButtonDown("SelectNextWisp"))
            player.SelectNextWisp();
        if (Input.GetButtonDown("SelectPreviousWisp"))
            player.SelectPreviousWisp();
    }
}
