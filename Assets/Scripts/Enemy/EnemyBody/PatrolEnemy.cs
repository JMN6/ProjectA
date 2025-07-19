using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : Enemy
{

    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;

    [field: SerializeField] public float PatrolSpeed { get; private set; } = 0.2f;

    [field: SerializeField] public Vector2 StartPos { get; private set; }
    [field: SerializeField] public Vector2 EndPos { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        StartPos = startPos.position;
        EndPos = endPos.position;
    }


    protected override void OnEnable()
    {
        currentHealth = maxHealth;
        SetAnimationTrigger(EnemyAnimation.Idle);

        Target = null;
        isParried = false;
        isDead = false;

        chaseRange.gameObject.SetActive(true);
    }
}
