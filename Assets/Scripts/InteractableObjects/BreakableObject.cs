using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour, IDamagalbe
{
    [SerializeField] private SpriteRenderer sp;
    [SerializeField] private Animation anim;
    [SerializeField] private AnimationClip clip;
   
    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("InteractableObject");
    }

    private void OnEnable()
    {
    }

    public void GetDamaged(int damage)
    {
        ShowEffect();
    }

    private void ShowEffect()
    {
    }
}
