using System;
using System.Collections;
using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public abstract class BaseCharacterController : MonoBehaviour, IBugsBunny, ITriggerReceiver, ICharacterPowerUpUser
    {
        [SerializeField] private bool isModelActive = false;
        public bool IsModelActive => isModelActive;

        [Header("Configuration")]
        [SerializeField] protected GlobalEnums.GameTeam currentTeam = GlobalEnums.GameTeam.None;
        [SerializeField] protected GameObject deathEffect;
        [SerializeField] protected GameObject spawnEffect;

        [Header("Status")]
        protected string characterID = "Not Assigned";
        public virtual bool LogThisClass { get; set; } = false;
        private bool IsModelInitialized = false;

        // Constants
        private const string RESPAWN_EFFECT = "RF_ReSpawrnEffect";
        private const string DEATH_EFFECT = "RF_DeathEffect";
        private const string CHARACTER_ROOT = "RF_CharacterRoot";
        private const string HEALTH_BAR = "RF_HealthBar";
        private const string ACTOR_COLLIDER = "RF_Collider";

        // Cached references
        protected GunHoister gunHoister;
        protected GameObject characterRootModel;
        protected GameObject healthBar;
        protected Animator characterAnimator;
        protected HealthHandler healthHandler;
        protected PowerUpBasket powerUpBasket;
        public GameObject actorCollider;

        private bool gunState;
        protected float currentMovementSpeed;

        public GlobalEnums.GameTeam CurrentTeam => currentTeam;
        public GlobalEnums.GameTeam Team => currentTeam;

        protected virtual void Initialize()
        {
            gunHoister = GetComponentInChildren<GunHoister>();
            healthHandler = GetComponent<HealthHandler>();
            powerUpBasket = GetComponent<PowerUpBasket>();
            characterAnimator = GetComponent<Animator>();

            deathEffect = Helper.FindDeepChild(transform, DEATH_EFFECT)?.gameObject;
            spawnEffect = Helper.FindDeepChild(transform, RESPAWN_EFFECT)?.gameObject;
            characterRootModel = Helper.FindDeepChild(transform, CHARACTER_ROOT)?.gameObject;
            healthBar = Helper.FindDeepChild(transform, HEALTH_BAR)?.gameObject;
            actorCollider = Helper.FindDeepChild(transform, ACTOR_COLLIDER)?.gameObject;

            healthHandler?.InitializeHealthSystem();

            var colliderHelper = actorCollider.GetComponentInChildren<CollisionTriggerHelper>();
            colliderHelper.AddReceiver(this);
            colliderHelper.InitializeReceivers();

            IsModelInitialized = true;
        }

        public abstract bool ActivateTheActor();

        protected void Activate(float delayBeforeCleanup = .1f, Action onActivated = null)
        {
            characterAnimator?.Play("Respwan");
            StartCoroutine(WaitForActivation(delayBeforeCleanup, onActivated));
        }

        protected virtual IEnumerator WaitForActivation(float delay, Action onActivated = null)
        {
            if (!IsModelInitialized)
                Initialize();

            if (spawnEffect != null)
                spawnEffect.SetActive(true);

            yield return new WaitForSeconds(delay);
            ActivateCharacter();
            currentMovementSpeed = GetInitialSpeed();
            onActivated?.Invoke();
        }

        protected virtual float GetInitialSpeed() => 0f;

        private void ActivateCharacter()
        {
            characterRootModel.SetActive(true);
            healthBar.SetActive(true);
            characterAnimator.enabled = true;
            powerUpBasket?.ResetPowerUp();
            healthHandler?.ResetHealth();
            actorCollider.SetActive(true);

            isModelActive = true;
        }

        public virtual void Deactivate()
        {
            gunState = false;
            actorCollider.SetActive(false);
            gunHoister.currentGun?.StopShoot();
            gunHoister.gameObject.SetActive(false);

            characterRootModel.SetActive(false);
            healthBar.SetActive(false);
            deathEffect.SetActive(false);
            spawnEffect.SetActive(false);

            characterAnimator.enabled = false;

            powerUpBasket?.ResetPowerUp();
            isModelActive = false;
        }

        public virtual void SetCurrentSpeed(float speed) => currentMovementSpeed = speed;

        protected virtual void SetPlayerTeam(GlobalEnums.GameTeam team)
        {
            currentTeam = team;

            var gun = gunHoister?.LoadGun(GlobalEnums.GunType.Gun1);
            Color gunColor = GetColorForTeam(team);

            if (LogThisClass)
                BugsBunny.Log($"Team: {team}, Gun Color: {gunColor}");

            TeamManager.Instance.RegisterTeam(team, gunColor, $"Team_{gunColor}");
            SetGunColor(gun, gunColor);
            gunHoister.gameObject.SetActive(false);
        }

        private static Color GetColorForTeam(GlobalEnums.GameTeam team)
        {
            return team switch
            {
                GlobalEnums.GameTeam.TeamBlue => Color.blue,
                GlobalEnums.GameTeam.TeamRed => Color.red,
                GlobalEnums.GameTeam.TeamYellow => Color.yellow,
                GlobalEnums.GameTeam.TeamGreen => Color.green,
                _ => Color.white,
            };
        }

        protected virtual void SetGunColor(Gun gun, Color color) => gun?.SetGunColor(color);

        public virtual void GunState(bool status)
        {
            gunState = status;
            if (status)
            {
                gunHoister.gameObject.SetActive(true);
                gunHoister.currentGun?.StartShoot();
            }
            else
            {
                gunHoister.currentGun?.StopShoot();
                gunHoister.gameObject.SetActive(false);
            }
        }

        protected virtual void ReduceHealth(float amount) => healthHandler?.TakeDamage(amount);

        #region Death Sequence

        public abstract bool KillTheActor();

        public void Kill(float delayBeforeCleanup, Action onKilled = null)
        {
            StartCoroutine(WaitForDeathEffect(delayBeforeCleanup, onKilled));
        }

        protected IEnumerator WaitForDeathEffect(float seconds, Action onComplete = null)
        {
            if (deathEffect != null)
            {
                deathEffect.SetActive(true);
                deathEffect.GetComponentInChildren<BloodBomb>()?.Blast();
            }

            yield return new WaitForSeconds(seconds);

            if (deathEffect != null) deathEffect.SetActive(false);
            if (spawnEffect != null) spawnEffect.SetActive(false);

            onComplete?.Invoke();
        }

        #endregion

        #region Power-Up Interface Implementation

        public float moveSpeed { get; set; }

        public abstract void OnPowerUpTriggered(GlobalEnums.PowerUpType triggeredPowerUp, float duration, float speedOffset);

        public virtual void OnPowerUpEnded(GlobalEnums.PowerUpType type)
        {
            // Optional: override if cleanup is needed per power-up
            if (LogThisClass)
                Debug.Log($"PowerUp ended: {type}");
        }

        #endregion

        #region Trigger Handling

        public void OnTriggerEnterReceived(Collider other) { }

        public void OnTriggerStayReceived(Collider other)
        {
            var hit = other.GetComponentInParent<BaseCharacterController>();
            if (hit == null || hit == this) return;
            if (hit.currentTeam == currentTeam) return;

            if (other.GetComponent<Bullet>())
            {
                ReduceHealth(GlobalStaticVariables.HealthHitRation);
            }
        }

        public void OnTriggerExitReceived(Collider other) { }

        #endregion
    }
}
