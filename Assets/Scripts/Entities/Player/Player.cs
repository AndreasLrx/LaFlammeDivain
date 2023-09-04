using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D body;
    private float invincibleCoolDown = 2.0f;
    private float invincibleCurrentCoolDown = 0.0f;
    private WispsGroup _wisps;
    private Vector2 _aimedDirection;
    private Vector2 _moveDirection;
    private Entity _entity;
    private Weapon weapon;
    private Animator animator;
    private bool onlyWeaponAttack = false;

    private bool isDead;

    private bool isNpc = false;

    public void setNpc() { isNpc = true; }


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
            if (Mathf.Abs(moveDirection.x) < float.Epsilon && Mathf.Abs(moveDirection.y) < float.Epsilon)
            {
                animator.SetBool("Walking", false);
            }
            else
            {
                animator.SetBool("Walking", true);
                animator.SetFloat("DirectionX", moveDirection.x);
                animator.SetFloat("DirectionY", moveDirection.y);
            }
        }
    }
    public Entity entity { get { return _entity; } }

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        weapon = GetComponentInChildren<Weapon>();
        _entity = GetComponent<Entity>();
        _wisps = Instantiate(PrefabManager.Instance.wispsGroup, transform).GetComponent<WispsGroup>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (invincibleCurrentCoolDown > float.Epsilon)
            invincibleCurrentCoolDown -= Time.deltaTime;
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
        if (isDead) return;

        if (invincibleCurrentCoolDown > float.Epsilon)
            return;

        invincibleCurrentCoolDown = invincibleCoolDown;
        if (!wisps.AbsorbDamage()) {
            isDead = true;
            if (isNpc) {
                // need to adapt wiht the DVD pathern
                // don't forget to reset at the restart of the game
                GetComponent<SpriteRenderer>().color = Color.red;
            } else {
                // Game over
                GameManager.Instance.GameOver();
            }
        }
    }

    public void setIsDead(bool _isDead) {
        isDead = _isDead;
    }

    void ResetColor()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
