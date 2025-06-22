using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public class BotAnimationController
    {
        private readonly Animator animator;
        private float startAnimTime;
        private float stopAnimTime;
        private float animationSpeedOffset;

        public BotAnimationController(Animator animator)
        {
            this.animator = animator;
        }

        public void Initialize(float startAnimTime, float stopAnimTime, float animationSpeedOffset)
        {
            this.startAnimTime = startAnimTime;
            this.stopAnimTime = stopAnimTime;
            this.animationSpeedOffset = animationSpeedOffset;
        }

        public void UpdateMovementAnimation(float velocity)
        {
            float adjustedSpeed = velocity * animationSpeedOffset;
            bool isMoving = adjustedSpeed > 0.1f;

            animator.SetBool("OnShooting", isMoving);
            animator.SetFloat("Blend", adjustedSpeed, 
                isMoving ? startAnimTime : stopAnimTime, 
                Time.deltaTime);
        }
    }
}