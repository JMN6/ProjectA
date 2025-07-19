using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour, IDamagalbe
{
    [SerializeField] private Animator anim;
    [SerializeField] private Collider2D col;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("InteractableObject");
    }

    private void OnEnable()
    {
        anim.SetBool("IsBroken", false);
        col.enabled = true;
    }

    public void GetDamaged(int damage)
    {
        ShowEffect();
    }

    private void ShowEffect()
    {
        anim.SetBool("IsBroken", true);
        col.enabled = false;
    }
}
