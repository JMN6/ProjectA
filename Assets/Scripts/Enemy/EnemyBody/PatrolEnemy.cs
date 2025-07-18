using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : Enemy
{

    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;

    public Transform StartPos { get { return startPos; } }
    public Transform EndPos { get { return endPos;} }
}
