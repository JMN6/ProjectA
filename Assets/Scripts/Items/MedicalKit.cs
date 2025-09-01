public class MedicalKit : ItemBase
{
    protected override void Awake()
    {
        base.Awake();

        Init(2, transform.position);
    }

    public override void Use()
    {
        GameManager.Instance.Player.IncreaseMaxHealth();
        GameManager.Instance.Player.ChangeHP(1);
    }
}