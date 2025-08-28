using UnityEngine;
using EnemeyFSM;
using UnityEngine.Rendering;

public class Enemy : Entity, IDamageable, IResetable
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
    [field: SerializeField] public AnimationPlayer anim { get; private set; }
    [field: SerializeField] public Movement move { get; private set; }
    [field: SerializeField] public WayFindingManager wayFindingManager { get; private set; }

    [SerializeField] private bool startFSMOnAwake = true;
    [SerializeField] private EnemyFSMManager fsmManager;


    public GameObject Target { get; set; }

    public bool isDead { get; set; }

    private Vector2 originPosition;

    protected virtual void Awake()
    {
        attackRange.size = attackRange.size * AttackRange;
        chaseRange.radius = SightRange;
        Speed = base.speed;

        originPosition = transform.position;
        chaseRange.gameObject.SetActive(true);

        if(startFSMOnAwake == true)
        {
            StartFSM();
        }
    }

    public virtual void StartFSM()
    {
        fsmManager.StartFSM();
    }

    #region Debug
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; 
        Gizmos.DrawWireSphere(chaseRange.transform.position, SightRange); 
    }
    #endregion

    public virtual void OnReset()
    {
        Init();

        currentHealth = maxHealth;
        anim.SetTrigger(EnemyAnimation.Idle.ToString());

        Target = null;
        isDead = false;

        gameObject.SetActive(true);
        chaseRange.gameObject.SetActive(true);

        fsmManager.OnReset();
    }

    public virtual void Init()
    {
        transform.position = originPosition;
    }

    public void GetDamaged(int damage, GameObject damageSource)
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

            AnalyticsSystem.Instance.OnEnemyKilled(this, damageSource);
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
}
