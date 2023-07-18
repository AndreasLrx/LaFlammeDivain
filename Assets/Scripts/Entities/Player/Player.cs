using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D body;
    private float invicibleCoolDown = 2.0f;
    private float invicibleCurrentCoolDown = 0.0f;
    private WispsGroup _wisps;
    private Vector2 _aimedDirection;
    private Vector2 _moveDirection;
    private Entity _entity;
    private Weapon weapon;
    private bool onlyWeaponAttack = false;


    public WispsGroup wisps { get { return _wisps; } }
    public Vector2 aimedDirection
    {
        get { return _aimedDirection; }
        set
        {
            _aimedDirection = value.normalized;
            weapon.PointerDirection = aimedDirection;
        }
    }
    public Vector2 moveDirection
    {
        get { return _moveDirection; }
        set
        {
            _moveDirection = value.normalized;
        }
    }
    public Entity entity { get { return _entity; } }

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        weapon = GetComponentInChildren<Weapon>();
        _entity = GetComponent<Entity>();
        _wisps = Instantiate(PrefabManager.Instance.wispsGroup, transform).GetComponent<WispsGroup>();
    }

    void Update()
    {
        if (invicibleCurrentCoolDown > float.Epsilon)
            invicibleCurrentCoolDown -= Time.deltaTime;
        body.velocity = moveDirection * entity.speed;
    }

    public void ToggleWeaponMode()
    {
        onlyWeaponAttack = !onlyWeaponAttack;
    }

    public void SelectNextWisp()
    {
        wisps.SelectNextStack();
    }

    public void SelectPreviousWisp()
    {
        wisps.SelectPreviousStack();
    }

    public void AddWisp(Wisp wisp)
    {
        wisp.owner = this;
        StartCoroutine(wisp.Attach(wisps));
    }

    public void Attack()
    {
        if (onlyWeaponAttack || !wisps.ActivateSelectedWisp())
            weapon?.Attack();
    }

    public void SecondaryAttack()
    {
        if (onlyWeaponAttack || !wisps.ActivateSelectedStack())
            weapon?.Attack();
    }

    public void TakeDamage()
    {
        if (invicibleCurrentCoolDown > float.Epsilon)
            return;
        invicibleCurrentCoolDown = invicibleCoolDown;
        if (!wisps.AbsorbDamage())
            GameManager.Instance.GameOver();
    }
}
