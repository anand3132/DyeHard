using System.Collections;
using UnityEngine;
using DG.Tweening;
namespace RedGaint
{
    public class ShieldTrigger : MonoBehaviour
    {
        public MeshRenderer SheidMesh;
        [Range(0,10)]
        public float shieldDuration;
        public void TriggerShield()
        {
            GetComponentInParent<HealthHandler>().OnPowerUpImpact(GetComponent<PowerUpHandle>().powerUpType,shieldDuration);
            StartCoroutine(TriggerShield(shieldDuration));
        }

        private IEnumerator TriggerShield(float duration)
        {
            SheidMesh.material.DOFloat(2f, "_Appear", 1f).SetEase(Ease.InSine, 2);
            yield return new WaitForSeconds(duration);
            SheidMesh.material.DOFloat(0f, "_Appear", 20f).SetEase(Ease.InSine, 2);
        }
    }
}