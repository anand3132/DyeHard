
using UnityEngine;

namespace RedGaint
{
    public class GameCoreElements : MonoBehaviour,IBugsBunny
    {
        public bool LogThisClass { get; } = false;

        public GameObject Player;
        public GolbalGameData golbalGameData;

        public GameObject GetPlayer()
        {
            return Player;
        }

        private void Awake()
        {
            if (golbalGameData == null)
            {
                BugsBunny.LogRed("Game Core ERROR:: GolbalGameData is null please attach the GolbalGameData..!!",this);
                return;
            }

            GlobalStaticVariables.LoadFromScriptableObject(golbalGameData);
        }

    }
}