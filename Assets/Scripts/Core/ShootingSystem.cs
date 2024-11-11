using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using nostra.input;

public class ShootingSystem : MonoBehaviour
{
    MovementInput input;
    [SerializeField] ParticleSystem inkParticle;
    [SerializeField] Transform parentController;
    [SerializeField] Transform splatGunNozzle;
    [SerializeField] CinemachineFreeLook freeLookCamera;
    CinemachineImpulseSource impulseSource;
    public VariableJoystick leftJoyStick;


    void Start()
    {
        input = GetComponent<MovementInput>();
        impulseSource = freeLookCamera.GetComponent<CinemachineImpulseSource>();
    }

    void Update() => CheckShooting();

    void CheckShooting()
    {
        bool pressing = false;
#if UNITY_EDITOR && !ONTEST_INPUT
        pressing = Input.GetMouseButton(0);
#else
        pressing = leftJoyStick.Direction.magnitude > 0.1;// NostraInput.GetAction("ShootButton", EActionEvent.Press);

#endif
        //if (pressing)
        //{
        //    VisualPolish();
        //    input.RotateToCamera(transform);
        //}

#if UNITY_EDITOR && !ONTEST_INPUT
        if (Input.GetMouseButtonDown(0))
            inkParticle.Play();
        else if (Input.GetMouseButtonUp(0))
            inkParticle.Stop();
#else
    if(leftJoyStick.Direction.magnitude>0.1)
        //if (NostraInput.GetAction("ShootButton", EActionEvent.Down))
            inkParticle.Play();
        if (leftJoyStick.Direction.magnitude < 0.1)

       // else if (NostraInput.GetAction("ShootButton", EActionEvent.Up))
            inkParticle.Stop();
#endif

        Vector3 angle = parentController.localEulerAngles;
        //parentController.localEulerAngles = new Vector3(Mathf.LerpAngle(angle.x, pressing ? RemapCamera(freeLookCamera.m_YAxis.Value, 0, 1, -25, 25) : 0, .3f), angle.y, angle.z);
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
