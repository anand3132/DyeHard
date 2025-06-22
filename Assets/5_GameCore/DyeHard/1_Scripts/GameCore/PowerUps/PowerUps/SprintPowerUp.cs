using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public class SprintPowerUp : MonoBehaviour, IPowerUp, IInjectUser
    {
        [Range(0f, 100f)] public float sprintDuration = 5f;
        [Range(0f, 100f)] public float speedMultiplier = 10f;

        private ICharacterPowerUpUser user;

        public void SetUser(ICharacterPowerUpUser user)
        {
            this.user = user;
        }

        public void Initialize() { }

        public void Activate()
        {
            if (user == null) return;

            user.OnPowerUpTriggered(GlobalEnums.PowerUpType.Sprint, sprintDuration, speedMultiplier);
            CoroutineRunner.Instance.StartCoroutine(RunSprint());
        }

        public void Cleanup()
        {
            user?.OnPowerUpEnded(GlobalEnums.PowerUpType.Sprint);
        }

        private System.Collections.IEnumerator RunSprint()
        {
            yield return new WaitForSeconds(sprintDuration);
            Cleanup();
            Destroy(gameObject);
        }
    }
}