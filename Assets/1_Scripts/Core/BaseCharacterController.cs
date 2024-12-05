using UnityEngine;

namespace RedGaint
{

    public abstract class BaseCharacterController : MonoBehaviour
    {
        protected GlobalEnums.GameTeam currentTeam=GlobalEnums.GameTeam.None;
        public GlobalEnums.GameTeam CurrentTeam => currentTeam;
        protected GunHoister gunHoister;
        public string characternID;
        private bool gunstate;
        protected virtual void SetGunColor(Gun gun, Color color)
        {
            if(gun)
                gun.SetGunColor(color);
            else
            {
                BugsBunny.LogRed("Cant able to get the Gun to set color");
            }
        }
        protected virtual void SetPlayerTeam(GlobalEnums.GameTeam team)
        {
            currentTeam = team;
            
            Gun gun= gunHoister.LoadGun(GlobalEnums.GunType.Gun1);
            
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
            return true;
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
            GetComponent<HealthBarLookAt>().TakeDamage(amount);
        }

    }//BaseCharacterController
}//RedGaint