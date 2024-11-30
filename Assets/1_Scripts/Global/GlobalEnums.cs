
using UnityEngine;
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
            SingleShot,
            DoubleShot
        }

        public enum GameTeam
        {
            None = 0,
            TeamRed,
            TeamBlue,
            TeamYellow,
            TeamGreen,
        }

        public enum PowerUpType
        {
            Sprint,
            Shield,
            Sludge,
            Bomb
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

    public static class GlobalStaticVariables
    {
        public static Color TeamRedColor { get; private set; }
        public static float GameSectionTime { get; private set; }
        public static float BotMaxHealth { get; private set; }
        public static float PlayerMaxHealth { get; private set; }

        public static void LoadFromScriptableObject(GolbalGameData data)
        {
            if (data == null)
            {
                Debug.LogError("GlobalStaticVariables: ScriptableObject data is null. Please assign the GameDataScriptableObject.");
                return;
            }

            TeamRedColor = data.teamRedColor;
            GameSectionTime = data.gameSectionTime;
            BotMaxHealth = data.botMaxHealth;
            PlayerMaxHealth = data.playerMaxHealth;

            Debug.Log("GlobalStaticVariables: Data loaded successfully.");
        }
    }

}