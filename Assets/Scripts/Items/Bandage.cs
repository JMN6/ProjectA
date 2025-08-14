public class Bandage : ItemBase
{
    public override void Use()
    {
        GameManager.Instance.Player.ChangeHP(1);
    }
}