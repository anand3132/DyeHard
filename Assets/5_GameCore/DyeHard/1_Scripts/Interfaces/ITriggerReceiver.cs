using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public interface ITriggerReceiver
    {
        void OnTriggerEnterReceived(Collider other);
        void OnTriggerStayReceived(Collider other);
        void OnTriggerExitReceived(Collider other);
    }
}