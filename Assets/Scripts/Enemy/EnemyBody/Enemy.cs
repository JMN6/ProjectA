using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemeyFSM;

public class Enemy : Entity, IDamagalbe
{
    public enum EnemyAnimation
    {
        Idle,
        Patrol,
        Run,
        Attack,
        IsDead
    }

    [field: SerializeField] public int Damage { get; private set; } // °ø°Ý·Â
    [field: SerializeField] public string Id { get; private set; }
    [field: SerializeField] public float AttackRange { get; private set; }
    [field: SerializeField] public float AttackDelay { get; private set; }
    [field: SerializeField] public float SightRange { get; private set; }

    public float Speed { get; private set; }

    [Header("Components")]
    [SerializeField] protected CircleCollider2D chaseRange;
    [field: SerializeField] public Rigidbody2D Rigid { get; private set; }
    [field: SerializeField] public SpriteRenderer spriteRenderer { get; private set; }
    [SerializeField] private Animator anim;

    [SerializeField] private EnemyFSMManager fsmManager;

    public GameObject Target { get; set; }

    public bool isParried { get; set; }

    public bool isDead { get; set; }

    private Vector2 originPosition;

    protected virtual void Awake()
    {
        chaseRange.radius = SightRange;
        Speed = base.speed;

        originPosition = transform.position;
    }

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        SetAnimationTrigger(EnemyAnimation.Idle);

        Target = null;
        isParried = false;

        transform.position = originPosition;
        chaseRange.gameObject.SetActive(true);

    }

    public void GetDamaged(int damage)
    {
        if(isDead == true)
        {
            return;
        }

        EffectManager.Instance.PlayParticle(0, transform.position + Vector3.up);

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        
        if(currentHealth <= 0)
        {
            fsmManager.ChangeState(FSMState.Death);
            isDead = true;
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
}
