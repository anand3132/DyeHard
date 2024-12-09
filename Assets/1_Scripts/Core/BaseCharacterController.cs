using System;
using System.Collections;
using UnityEngine;
namespace RedGaint
{

    public abstract class BaseCharacterController : MonoBehaviour
    {
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
                BugsBunny.LogRed("Cant able to get the Gun to set color");
            }
        }

        protected  virtual void Start() { }


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
            SetGunColor(gun,gunColor);
        }
        
        public void OnBulletHit(Collider other)
        {
            if(!gunstate)return;
            BaseCharacterController otherController = other.GetComponent<BaseCharacterController>();
            if (otherController.CurrentTeam != CurrentTeam)
            {
                otherController.ReduceHealth(GlobalStaticVariables.HealthHitRation);
                BugsBunny.LogRed("OnBulletHit: "+otherController.characternID);
            }
        }

        public  virtual bool KillTheActor()
        {
            BugsBunny.LogRed("-------------------------KILL THE ACTOR--------------------------------");
            if (GetComponent<BotController>() != null)
            {
                var _bot = GetComponent<BotController>();
                BotGenerator.instance.AddToReSpawnList(_bot);
                if(deadthEffect!=null)
                    deadthEffect.SetActive(true);
                StartCoroutine(WaitForDeadthEffect(.1f));
            }

            // if(GetComponent<PlayerController>() !=null)

            return true;
        }

        private IEnumerator WaitForDeadthEffect(float seconds)
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