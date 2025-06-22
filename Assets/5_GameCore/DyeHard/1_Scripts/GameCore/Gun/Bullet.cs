using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public class Bullet : MonoBehaviour, IBugsBunny
    {
        private float lastTriggerTime = 0f;
        private const float triggerCooldown = .3f;


        // private void OnTriggerStay(Collider other)
        // {
        //     if (Time.time - lastTriggerTime < triggerCooldown)
        //     {
        //      //   BugsBunny.LogRed("on trigger cool down hit", this);
        //         //return;
        //     }
        //
        //     // if(other.GetComponent<PlayerController>())
        //     //     BugsBunny.LogRed("Bullet hit",this);
        //     
        //     if (other.GetComponent<BaseCharacterController>())
        //     {
        //         lastTriggerTime = Time.time;
        //         GetComponentInParent<BaseCharacterController>().OnBulletHit(other);
        //     }
        // }

        public bool LogThisClass { get; } = false;
    }
}