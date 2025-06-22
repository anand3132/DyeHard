using UnityEngine;
using DG.Tweening;

namespace RedGaint.Games.DyeHard
{
    public class ShieldPowerUp : MonoBehaviour, IPowerUp, IInjectUser
    {
        public MeshRenderer shieldMesh;
        [Range(0f, 10f)] public float shieldDuration = 5f;

        private ICharacterPowerUpUser user;

        public void SetUser(ICharacterPowerUpUser user)
        {
            this.user = user;
        }

        public void Initialize() { }

        public void Activate()
        {
            GetComponentInParent<HealthHandler>()?.OnPowerUpImpact(GlobalEnums.PowerUpType.Shield, shieldDuration);
            StartCoroutine(ShieldCoroutine());
        }

        public void Cleanup()
        {
            // Optionally fade out or disable shield visuals here
        }

        private System.Collections.IEnumerator ShieldCoroutine()
        {
            shieldMesh.material.DOFloat(2f, "_Appear", 1f).SetEase(Ease.InSine);
            yield return new WaitForSeconds(shieldDuration);
            shieldMesh.material.DOFloat(0f, "_Appear", 2f).SetEase(Ease.InSine);
            Cleanup();
            Destroy(gameObject);
        }
    }
}