using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : Enemy
{

    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;

    [field: SerializeField] public float PatrolSpeed { get; private set; } = 0.2f;

    public Transform StartPos { get { return startPos; } }
    public Transform EndPos { get { return endPos;} }
}
