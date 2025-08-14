public class MedicalKit : ItemBase
{
    public override void Use()
    {
        GameManager.Instance.Player.IncreaseMaxHealth();
    }
}