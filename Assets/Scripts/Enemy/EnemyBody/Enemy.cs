using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemeyFSM;

public class Enemy : Entity, IDamagalbe
{
    public enum EnemyAnimation
    {
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
    [SerializeField] private CircleCollider2D chaseRange;
    [field: SerializeField] public Rigidbody2D Rigid { get; private set; }
    [field: SerializeField] public SpriteRenderer spriteRenderer { get; private set; }
    [SerializeField] private Animator anim;

    [SerializeField] private EnemyFSMManager fsmManager;

    public GameObject Target { get; set; }

    private void Start()
    {
        chaseRange.radius = SightRange;
        Speed = base.speed;
    }

    public void GetDamaged(int damage)
    {
        EffectManager.Instance.PlayParticle(0, transform.position);

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        
        if(currentHealth <= 0)
        {
            fsmManager.ChangeState(FSMState.Death);
        }
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
