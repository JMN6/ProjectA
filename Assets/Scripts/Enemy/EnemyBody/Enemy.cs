using UnityEngine;
using EnemeyFSM;

public class Enemy : Entity, IDamagalbe, IResetable
{
    public enum EnemyAnimation
    {
        Idle,
        Patrol,
        Run,
        Attack,
        IsDead,
        Hurt
    }

    [field: SerializeField] public int Damage { get; private set; } // °ø°Ý·Â
    [field: SerializeField] public string Id { get; private set; }
    [field: SerializeField] public float AttackRange { get; private set; }
    [field: SerializeField] public float AttackDelay { get; private set; }
    [field: SerializeField] public float SightRange { get; private set; }

    public float Speed { get; private set; }

    [Header("Components")]
    [SerializeField] protected CircleCollider2D chaseRange;
    [SerializeField] protected BoxCollider2D attackRange;
    [field: SerializeField] public Rigidbody2D Rigid { get; private set; }
    [field: SerializeField] public SpriteRenderer spriteRenderer { get; private set; }
    [SerializeField] private Animator anim;

    [SerializeField] private EnemyFSMManager fsmManager;

    [field: SerializeField] public AudioClip WalkSFX { get; private set; }
    [field: SerializeField] public AudioClip IdleSFX { get; private set; }
    [field: SerializeField] public AudioClip AttackSFX { get; private set; }
    [field: SerializeField] public AudioClip DeadSFX { get; private set; }
    [field: SerializeField] public AudioClip HurtSFX { get; private set; }
    [field: SerializeField] public AudioClip HurtSFX2 { get; private set; }

    public GameObject Target { get; set; }

    public bool isParried { get; set; }

    public bool isDead { get; set; }

    private Vector2 originPosition;

    private float timer;

    protected virtual void Awake()
    {
        attackRange.size = attackRange.size * AttackRange;
        chaseRange.radius = SightRange;
        Speed = base.speed;

        originPosition = transform.position;
        chaseRange.gameObject.SetActive(true);

        fsmManager.StartFSM();
    }

    private void Update()
    {
        timer += Time.timeScale;

        if (timer > 0.2f)
        {
            timer = 0;
            Speed = 6 / Speed;
        }
    }

    public virtual void OnReset()
    {
        Init();

        currentHealth = maxHealth;
        SetAnimationTrigger(EnemyAnimation.Idle);

        Target = null;
        isParried = false;
        isDead = false;

        gameObject.SetActive(true);
        chaseRange.gameObject.SetActive(true);

        fsmManager.OnReset();
    }

    public virtual void Init()
    {
        transform.position = originPosition;
    }

    public void GetDamaged(int damage)
    {
        if(isDead == true)
        {
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);

        if (currentHealth <= 0)
        {
            fsmManager.ChangeState(FSMState.Death);
            isDead = true;
        }
        else
        {
            fsmManager.ChangeState(FSMState.Hurt);
        }
    }

    public void StopLookingForPlayer()
    {
        chaseRange.gameObject.SetActive(false);
    }

    public void DestroySelf()
    {
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

    public void SetAnimationTrigger(EnemyAnimation _trigger)
    {
        anim.SetTrigger(_trigger.ToString());
    }

    public float GetCurrentAnimationLength()
    {
        return anim.GetCurrentAnimatorStateInfo(0).length;
    }
}
