public class AmmoBox : ItemBase
{
    public override void Use()
    {
        GameManager.Instance.Aim.CurGun.ChangeAmmo(3);
    }
}