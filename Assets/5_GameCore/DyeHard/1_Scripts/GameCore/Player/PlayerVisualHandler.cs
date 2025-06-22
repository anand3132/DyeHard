using System.Collections;
using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public class PlayerVisualHandler
    {
        private GameObject gunHoister, characterRootModel, healthBar, deathEffect, spawnEffect;
        private MonoBehaviour owner;

        public PlayerVisualHandler(MonoBehaviour owner, GameObject gunHoister, GameObject characterRootModel, GameObject healthBar, GameObject deathEffect, GameObject spawnEffect)
        {
            this.owner = owner;
            this.gunHoister = gunHoister;
            this.characterRootModel = characterRootModel;
            this.healthBar = healthBar;
            this.deathEffect = deathEffect;
            this.spawnEffect = spawnEffect;
        }

        public void Reset()
        {
            gunHoister.SetActive(false);
            characterRootModel.SetActive(false);
            healthBar.SetActive(false);
            deathEffect.SetActive(false);
            spawnEffect.SetActive(false);
            owner.StartCoroutine(WaitAndActivate(0.5f));
        }

        private IEnumerator WaitAndActivate(float seconds)
        {
            spawnEffect.SetActive(true);
            yield return new WaitForSeconds(seconds);
            gunHoister.SetActive(true);
            characterRootModel.SetActive(true);
            healthBar.SetActive(true);
        }
    }
}