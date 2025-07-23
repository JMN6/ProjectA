namespace Path
{
    public static class SFXPath
    {
        public const string Base = "Sounds/SFX";
    }

    public static class GunSFX
    {
        private static readonly string Base = $"{SFXPath.Base}/Guns";

        public static class SniperSFX
        {
            public static readonly string Shoot = $"{Base}/Sniper/Shoot";
            public static readonly string Reload = $"{Base}/Sniper/Reload";
            public static readonly string Empty = $"{Base}/Sniper/Empty";
        }
    }

    public static class ZombieSFX
    {
        private static readonly string Base = $"{SFXPath.Base}/Zombie";

        public static readonly string Attack = $"{Base}/Attack";
        public static readonly string Walk = $"{Base}/Walk";
        public static readonly string Idle = $"{Base}/Idle";
        public static readonly string Hurt = $"{Base}/Hurt";
        public static readonly string Dead = $"{Base}/Dead";
    }

    public static class PlayerSFX
    {
        private static readonly string Base = $"{SFXPath.Base}/Player";

        public static readonly string Walk = $"{Base}/Walk";
        public static readonly string Jump = $"{Base}/Jump";
        public static readonly string Parry = $"{Base}/Parry";
        public static readonly string Dead = $"{Base}/Dead";
    }
}