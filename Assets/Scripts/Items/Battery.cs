public class Battery : ItemBase
{
    protected override void Awake()
    {
        base.Awake();

        Init(3, transform.position);
    }
    public override void Use()
    {
        GameManager.Instance.Aim.ScaleFlashLightRadius(1.1f);
    }
}