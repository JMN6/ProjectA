public class Bandage : ItemBase
{
    protected override void Awake()
    {
        base.Awake();

        Init(0, transform.position);
    }

    public override void Use()
    {
        GameManager.Instance.Player.ChangeHP(1);
    }
}