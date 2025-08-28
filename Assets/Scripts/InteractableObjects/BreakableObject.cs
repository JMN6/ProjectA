using UnityEngine;

public class BreakableObject : MonoBehaviour, IDamageable, IResetable
{
    [SerializeField] private Animator anim;
    [SerializeField] private Collider2D col;

    private static readonly int IsBrokenAnimID = Animator.StringToHash("IsBroken");

    protected virtual void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("InteractableObject");
    }

    public virtual void OnReset()
    {
        anim.SetBool(IsBrokenAnimID, false);
        col.enabled = true;
    }

    public virtual void GetDamaged(int damage, GameObject damageSource)
    {
        ShowEffect();
    }

    private void ShowEffect()
    {
        anim.SetBool(IsBrokenAnimID, true);
        col.enabled = false;
    }
}
