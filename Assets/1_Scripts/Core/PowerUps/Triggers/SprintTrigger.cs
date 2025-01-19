using System.Collections;
using UnityEngine;

namespace RedGaint
{
    public class SprintTrigger : MonoBehaviour
    {
        [Range(0, 100f)] public float SpritDuration = 5f;
        [Range(0f, 100f)] public float SpritSpeed =10f;
        public void TriggerSpeedRun()
        {
            GetComponentInParent<BaseCharacterController>().OnPowerUpTriggered(GlobalEnums.PowerUpType.Sprint,SpritDuration,SpritSpeed);
        }

    }
}