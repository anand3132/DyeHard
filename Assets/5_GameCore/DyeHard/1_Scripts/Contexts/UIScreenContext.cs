namespace RedGaint.Games.DyeHard.UI
{
    public class UIScreenContext
    {
        // Optionally include common fields
        public System.Action onComplete;
    }
    public class RespawnScreenContext : UIScreenContext
    {
        public float respawnTime;
        public System.Action onRespawnComplete;
    }
    public class GameEndScreenContext : UIScreenContext
    {
        public string winningTeamMessage;
        public System.Action onExitClicked;
    }
    
    public class HUDScreenContext : UIScreenContext
    {
        public UnityEngine.Sprite powerUpIcon;
    }

}//RedGaint.UI
