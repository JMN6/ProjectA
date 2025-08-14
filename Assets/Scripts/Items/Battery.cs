public class Battery : ItemBase
{
    public override void Use()
    {
        GameManager.Instance.Aim.ScaleFlashLightRadius(1.1f);
    }
}