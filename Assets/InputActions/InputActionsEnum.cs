namespace Game.Input
{
    [System.Flags]
    public enum TriggerFlag
    {
        Started = 1 << 0,
        Performed = 1 << 1,
        Canceled = 1 << 2,
    }

    public enum Maps
    {
        Player,
        Aim,
        UI,
    }

    public enum Actions
    {
        Move,
        Jump,
        Interact,
        Parry,
        Snipe,
        Shoot,
        Reload,
        ESC,
        Skip,
        Title,
    }
}
