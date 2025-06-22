using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public interface ICollisionReceiver
    {
        void OnCollisionEnterReceived(Collision collision);
        void OnCollisionStayReceived(Collision collision);
        void OnCollisionExitReceived(Collision collision);
    }

}