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
        UI,
    }

    public enum Actions
    {
        Move,
        Look,
        Fire,
        Navigate,
        Submit,
        Cancel,
        Point,
        Click,
        ScrollWheel,
        MiddleClick,
        RightClick,
        TrackedDevicePosition,
        TrackedDeviceOrientation,
        //임시로 만든 내용
        Snipe,
        Shoot,
        Reload,
        Interact,
        Parry,
        Jump,
    }
}
