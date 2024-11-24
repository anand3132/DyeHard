using UnityEngine;
using DG.Tweening;
using Cinemachine;
using System;

namespace RedGaint
{
    public class ShootingSystem : MonoBehaviour
    {
        [SerializeField] ParticleSystem inkParticle;
        [SerializeField] Transform parentController;
        [SerializeField] Transform splatGunNozzle;
        CinemachineImpulseSource impulseSource;

        //void Update() => CheckShooting();
        private void OnEnable()
        {
            InputHandler.instance.HandOnLeftJoystick += OnTriggerOn;
            InputHandler.instance.ReleaseLeftJoystick += OnTriggerOFF;

        }


        private void OnDisable()
        {
            InputHandler.instance.HandOnLeftJoystick -= OnTriggerOn;
            InputHandler.instance.ReleaseLeftJoystick -= OnTriggerOFF;

        }

        void OnTriggerOn(Vector2 direction)
        {
            Debug.Log("Shooting....");
            inkParticle.Play();

            bool pressing = InputHandler.instance.GetLeftJoystickDirection().magnitude > 0.1;

            if (direction.magnitude > 0.1)
                if (direction.magnitude < 0.1)
                    inkParticle.Stop();

            Vector3 angle = parentController.localEulerAngles;
            //parentController.localEulerAngles = new Vector3(Mathf.LerpAngle(angle.x, pressing ? RemapCamera(freeLookCamera.m_YAxis.Value, 0, 1, -25, 25) : 0, .3f), angle.y, angle.z);
        }

        void OnTriggerOFF()
        {
            inkParticle.Stop();  
        }

        void VisualPolish()
        {
            if (!DOTween.IsTweening(parentController))
            {
                parentController.DOComplete();
                Vector3 localPos = parentController.localPosition;
                parentController.DOLocalMove(localPos - new Vector3(0, 0, .2f), .03f)
                    .OnComplete(() => parentController.DOLocalMove(localPos, .1f).SetEase(Ease.OutSine));

                impulseSource.GenerateImpulse();
            }

            if (!DOTween.IsTweening(splatGunNozzle))
            {
                splatGunNozzle.DOComplete();
                splatGunNozzle.DOPunchScale(new Vector3(0, 1, 1) / 1.5f, .15f, 10, 1);
            }
        }

        float RemapCamera(float value, float from1, float to1, float from2, float to2) =>
            (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}