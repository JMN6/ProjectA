using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : Enemy
{

    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;

    [field: SerializeField] public float PatrolSpeed { get; private set; } = 0.2f;
    public Vector2 StartPos { get; private set; }
    public Vector2 EndPos { get; private set; }

    protected override void Awake()
    {
        StartPos = startPos.position;
        EndPos = endPos.position;
        base.Awake();
    }

    private void Start()
    {
    }
}
