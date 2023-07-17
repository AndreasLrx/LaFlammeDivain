using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{
    [SerializeField] private LayerMask enemyLayers;
    Rigidbody2D body;

    float horizontal;
    float vertical;
    float moveLimiter = 0.7f;
    public float runSpeed = 10.0f;
    public float invisibleCoolDown = 2.0f;
    float invisibleCurrentCoolDown = 0.0f;
    public GameObject wispsGroupPrefab;
    private bool onlyWeaponAttack = false;
    private Weapon weapon;

    private Entity _entity;
    public Entity entity { get { return _entity; } }

    protected override void Awake()
    {
        base.Awake();
        weapon = GetComponentInChildren<Weapon>();
        body = GetComponent<Rigidbody2D>();
        _entity = GetComponent<Entity>();
        Instantiate(wispsGroupPrefab, transform);
    }

    void Update()
    {
        weapon.PointerDirection = AimedDirection();
        // Gives a value between -1 and 1
        horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        vertical = Input.GetAxisRaw("Vertical"); // -1 is down

        if (Input.GetButtonDown("Attack"))
            Attack();
        if (Input.GetButtonDown("ToggleWeaponMode"))
            ToggleWeaponMode();

        if (Input.GetButtonDown("SelectNextWisp"))
            GetWisps().SelectNextWisp();
        if (Input.GetButtonDown("SelectPreviousWisp"))
            GetWisps().SelectPreviousWisp();

        if (invisibleCurrentCoolDown >= float.Epsilon)
            invisibleCurrentCoolDown -= Time.deltaTime;
    }

    public void ToggleWeaponMode()
    {
        onlyWeaponAttack = !onlyWeaponAttack;
    }

    public WispsGroup GetWisps()
    {
        return gameObject.GetComponentInChildren<WispsGroup>();
    }

    void FixedUpdate()
    {
        if (horizontal != 0 && vertical != 0) // Check for diagonal movement
        {
            // limit movement speed diagonally, so you move at 70% speed
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }

        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }

    public Vector2 AimedDirection()
    {
        return ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized;
    }

    public void AddWisp(Wisp wisp)
    {
        wisp.owner = entity;
        GetWisps().AddWisp(wisp);
        StartCoroutine(wisp.OnAttach());
    }

    private void Attack()
    {
        Wisp wisp = GetWisps().GetSelectedWisp();
        if (!onlyWeaponAttack && wisp != null && wisp.IsActivable())
            StartCoroutine(wisp.Activate());
        else
            gameObject.GetComponentInChildren<Weapon>().Attack();
    }

    public void TakeDamage()
    {
        if (invisibleCurrentCoolDown > float.Epsilon)
            return;
        Wisp wisp = GetWisps().GetSelectedWisp();
        invisibleCurrentCoolDown = invisibleCoolDown;
        if (wisp != null)
        {
            GetWisps().DetachWisp(wisp);
            Destroy(wisp.gameObject);
        }
        else
            GameManager.Instance.GameOver();
    }
}
