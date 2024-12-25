
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
            DoubleShot,
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
            //None,
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

    // public class ColorFillAmount
    // {
    //     public Color PlayerColor;  // Color of the player
    //     public int FillAmount;     // The amount of fill for that color
    //
    //     // Constructor to initialize the color and fill amount
    //     public ColorFillAmount(Color color)
    //     {
    //         PlayerColor = color;
    //         FillAmount = 0; // Initialize the fill amount to 0
    //     }
    //
    //     // Optionally, add methods to update the fill amount or perform other operations
    //     public void AddFillAmount(int amount)
    //     {
    //         FillAmount += amount;
    //     }
    //
    //     // A method to reset fill amount if needed
    //     public void ResetFillAmount()
    //     {
    //         FillAmount = 0;
    //     }
    // }


    public static class GlobalStaticVariables
    {
        public static Color TeamRedColor { get; private set; }
        public static float GameSectionTime { get; private set; }
        public static float BotMaxHealth { get; private set; }
        public static float PlayerMaxHealth { get; private set; }
        public static readonly float HealthHitRation = 20f;

        public static void LoadFromScriptableObject(GolbalGameData data)
        {
            if (data == null)
            {
              //  BugsBunny.LogRed("GlobalStaticVariables: ScriptableObject data is null. Please assign the GameDataScriptableObject.",);
                return;
            }

            TeamRedColor = data.teamRedColor;
            GameSectionTime = data.gameSectionTime;
            BotMaxHealth = data.botMaxHealth;
            PlayerMaxHealth = data.playerMaxHealth;

           // BugsBunny.Log("GlobalStaticVariables: Data loaded successfully.");
        }
    }

}//RedGaint