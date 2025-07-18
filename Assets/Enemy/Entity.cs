using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected int maxHealth; // 최대 생명력
    [SerializeField] protected int currentHealth; // 현재 생명력
    [SerializeField] protected float speed; // 이동속도
    [SerializeField] protected int damage; // 공격력
}
