
namespace RedGaint
{

    public static class GlobalEnums
    {
        public enum Mode
        {
            Random,
            Sequence,
            Stack,
            Shuffle,
            RoundRobin,
            ReverseSequence,
            Cluster,
        }
        public enum BotType
        {
            Max,
            Attacker,
            Defender,
            Runner,
            Random,
            Balanced
        }
        public enum LogLevel
        {
            FullLog,
            PartialLog,
            ErrorOnly
        }
        public enum GunType
        {
            Gun1,
            Gun2,
            Gun3,
            Gun4, 
            Gun5,
        }

        public enum CheckPointType
        {
            None,
            WayPoint,
            DefendPoint,
            Destination,
            SpawnPoint
        }
        public enum DifficultyTiers
        {
            Easy,
            Normal,
            Hard
        }
        public enum RotationMode
        {
            RandomMode,
            SineWaveMode
        }
    }
}