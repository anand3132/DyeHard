using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace RedGaint
{
    public abstract class BaseCharacterController : MonoBehaviour,IBugsBunny
    {
        private BaseCharacterController targetLockBy = null;
        public virtual bool LogThisClass { get; set; } = false; 
        protected GlobalEnums.GameTeam currentTeam=GlobalEnums.GameTeam.None;
        public GlobalEnums.GameTeam CurrentTeam => currentTeam;
        protected GunHoister gunHoister;
        public string characternID="Not Assigned";
        private bool gunstate;
        [SerializeField] protected GameObject deadthEffect;
        [SerializeField] protected GameObject spawnEffect;

        protected virtual void SetGunColor(Gun gun, Color color)
        {
            if(gun)
                gun.SetGunColor(color);
            else
            {
                BugsBunny.LogRed("Cant able to get the Gun to set color",this);
            }
        }

        public void ResetAll()
        {
            gameObject.GetComponent<PowerUpBasket>().ResetPowerUp();
            gameObject.GetComponent<HealthHandler>().ResetHealth();
            BugsBunny.LogRed("Resetting the player.......",this);
        }

        public bool TryTargetLock(BaseCharacterController lockTarget)
        {
            if(targetLockBy==null)
                targetLockBy = lockTarget;
            return targetLockBy != null;
        }

        public bool IsTargetLocked()
        {
            return targetLockBy != null;
        }
        public bool ReleaseLocked(BaseCharacterController lockTarget)
        {
            if (targetLockBy == lockTarget)
            {
                targetLockBy = null;
                return true;
            }
            return false;
        }

        protected virtual void Start()
        {
            GetComponent<HealthHandler>().InitializeHealthSystem();
        }


        protected virtual void SetPlayerTeam(GlobalEnums.GameTeam team)
        {
            currentTeam = team;
            
            Gun gun= gunHoister.LoadGun(GlobalEnums.GunType.Gun1);
            
           //need to shift from here------------------------------------#color 
            Color gunColor=Color.white;
            switch (team)
            {
                case GlobalEnums.GameTeam.TeamBlue:
                    gunColor=Color.blue;
                    break;
                case GlobalEnums.GameTeam.TeamRed:
                    gunColor=Color.red;
                    break;
                case GlobalEnums.GameTeam.TeamYellow:
                    gunColor=Color.yellow;
                    break;
                case GlobalEnums.GameTeam.TeamGreen:
                    gunColor=Color.green;
                    break;
            }
            BugsBunny.Log("Getting team color : "+team+"  gun color : "+gunColor);
            TeamManager.Instance.RegisterTeam(team, gunColor, "Team_"+gunColor);
            SetGunColor(gun,gunColor);
        }

        public abstract void OnPowerUpTriggered(GlobalEnums.PowerUpType triggeredPowerUp,float duration,float speedOffset);
        public void OnBulletHit(Collider other)
        {
            if(!gunstate)return;
            BaseCharacterController otherController = other.GetComponent<BaseCharacterController>();
            if (otherController.CurrentTeam != CurrentTeam)
            {
                otherController.ReduceHealth(GlobalStaticVariables.HealthHitRation);
                BugsBunny.LogRed("OnBulletHit: "+otherController.characternID,this);
            }
        }

        public abstract  bool KillTheActor();

        protected IEnumerator WaitForDeadthEffect(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            gameObject.SetActive(false);
        } 

        protected virtual void GunState(bool status)
        {
            if (status)
            {
                gunHoister.currentGun.StartShoot();
            }
            else
            {
                gunHoister.currentGun.StopShoot();
            }

            gunstate = status;
        }

        protected virtual void ReduceHealth(float amount)
        {
            GetComponent<HealthHandler>().TakeDamage(amount);
        }

    }//BaseCharacterController
}//RedGaint