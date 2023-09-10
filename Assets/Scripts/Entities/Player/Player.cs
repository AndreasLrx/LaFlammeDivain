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
    private Animator animator;
    private bool onlyWeaponAttack = false;
    [SerializeField] private AudioSource attackSound = null;
    // [SerializeField] private AudioSource runningSound;
    [SerializeField] private AudioSource SwitchNextWispSound = null;
    [SerializeField] private AudioSource SwitchPreviousWispSound = null;
    [SerializeField] private AudioSource DeathSound = null;
    [SerializeField] private AudioSource WispAbsorbSound = null;

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
        SwitchNextWispSound.Play();
        wisps.SelectNextStack();
    }

    public void SelectPreviousWisp()
    {
        SwitchPreviousWispSound.Play();
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
        {
            weapon?.Attack();
            attackSound.pitch = Random.Range(0.9f, 1.1f);
            attackSound.Play();
        }
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
        GetComponent<SpriteRenderer>().color = Color.red;
        Invoke("ResetColor", 0.1f);
        if (!wisps.AbsorbDamage())
        {
            GameManager.Instance.GameOver();
            if (DeathSound != null && DeathSound.clip != null)
                DeathSound.Play();
        }
        else
            WispAbsorbSound.Play();

    }

    void ResetColor()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    // TODO: DeathSound when player dies
}
