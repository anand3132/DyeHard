using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public interface IPowerUp
    {
        void Initialize();
        void Activate();
        void Cleanup();
    }
}