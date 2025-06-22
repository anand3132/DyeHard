using RedGaint.Games.DyeHard.UI;
using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public class PowerUpHandler
    {
        private float baseSpeed;
        private float sprintSpeed;
        private PowerUpBasket powerUpBasket;
        private System.Action<float> setSpeedCallback;

        public PowerUpHandler(float baseSpeed, System.Action<float> setSpeedCallback, PowerUpBasket basket)
        {
            this.baseSpeed = baseSpeed;
            this.setSpeedCallback = setSpeedCallback;
            this.powerUpBasket = basket;
        }
        
        public void TriggerPowerUp()
        {
            if (powerUpBasket == null || !powerUpBasket.IsPowerUpAvilable())
                return;

            powerUpBasket.TriggerPowerUp();

            if (UXController.Instance.GetCurrentScreen() == UIScreen.HUD &&
                UXController.Instance.TryGetScreen<HUDScreenUI>(UIScreen.HUD, out var hudUI))
            {
                hudUI.SetPowerUpIcon(null);
            }
        }

        public void TriggerSprint(float speedOffset, float duration)
        {
            if (speedOffset <= 0 || duration <= 0) return;

            sprintSpeed = baseSpeed + speedOffset;
            setSpeedCallback?.Invoke(sprintSpeed);
            Timer(duration, () => setSpeedCallback?.Invoke(baseSpeed));
        }

        private void Timer(float time, System.Action onComplete)
        {
            GamePlayManager.Instance.StartCoroutine(WaitAndCall(time, onComplete));
        }

        private System.Collections.IEnumerator WaitAndCall(float time, System.Action callback)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }
    }
}