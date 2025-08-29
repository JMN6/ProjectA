public class AmmoBox : ItemBase
{
    public override void Use()
    {
        GameManager.Instance.Aim.ChangeAmmo(1);
    }
}