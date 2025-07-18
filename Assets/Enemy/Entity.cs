using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected int maxHealth; // �ִ� �����
    [SerializeField] protected int currentHealth; // ���� �����
    [SerializeField] protected float speed; // �̵��ӵ�
    [SerializeField] protected int damage; // ���ݷ�
}
