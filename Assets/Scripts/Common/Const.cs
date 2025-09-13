using System;

namespace Path
{
    public static class SFX
    {
        public const string Base = "Sounds/SFX";

        public static class Gun
        {
            public static class SniperSFX
            {
                public static readonly string Shoot = $"{Base}/Guns/Sniper/Shoot";
                public static readonly string Reload = $"{Base}/Guns/Sniper/Reload";
                public const int ReloadSoundsCount = 2;
                public static readonly string Empty = $"{Base}/Guns/Sniper/Empty";
            }
        }

        public static class Zombie
        {
            private static readonly string Base = $"{SFX.Base}/Zombie";

            public static readonly string Attack = $"{Base}/Attack";
            public const int AttackSoundsCount = 2;
            public static readonly string Walk = $"{Base}/Walk";
            public static readonly string Idle = $"{Base}/Idle";
            public static readonly string Hurt = $"{Base}/Hurt";
            public const int HurtSoundsCount = 2;
            public static readonly string Dead = $"{Base}/Dead";
            public const int DeadSoundsCount = 2;
        }
        public static class Player
        {
            private static readonly string Base = $"{SFX.Base}/Player";

            public static readonly string Walk = $"{Base}/Walk";
            public const int WalkSoundsCount = 2;
            public static readonly string Jump = $"{Base}/Jump";
            public const int JumpSoundsCount = 2;
            public static readonly string Parry = $"{Base}/Parry";
            public static readonly string ParrySuccess = $"{Base}/ParrySuccess";
            public static readonly string Dead = $"{Base}/Dead";
        }

        public static class BreakableObj
        {
            private static readonly string Base = $"{SFX.Base}/BreakableObj";

            public static readonly string OakCask = $"{Base}/OakCask";
        }
    }

    public static class Data
    {
        public const string Base = "Data";

        public static readonly string Items = $"{Base}/Items";
    }

    public static class Prefab
    {
        public const string Base = "Prefabs";

        public static readonly string Player = $"{Base}/Player";
        public static readonly string Aim = $"{Base}/Aim";
    }
}