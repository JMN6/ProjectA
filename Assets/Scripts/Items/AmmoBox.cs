public class AmmoBox : ItemBase
{
    protected override void Awake()
    {
        base.Awake();

        Init(1, transform.position);
    }

    public override void Use()
    {
        GameManager.Instance.Aim.ChangeAmmo(1);
    }
}