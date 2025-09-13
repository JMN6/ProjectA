using UnityEngine;

public class OakCask : BreakableObject
{
    public override void GetDamaged(int damage, GameObject damageSource)
    {
        base.GetDamaged(damage, damageSource);

        SoundManager.Instance.PlaySFX(Path.SFX.BreakableObj.OakCask);
    }
}
