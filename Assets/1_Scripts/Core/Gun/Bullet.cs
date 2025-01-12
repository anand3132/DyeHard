using System;
using System.Collections;
using System.Collections.Generic;
using RedGaint;
using UnityEngine;


public class Bullet : MonoBehaviour,IBugsBunny
{
    private float lastTriggerTime = 0f; 
    private const float triggerCooldown = .1f;
    
    private void OnTriggerStay(Collider other)
    {
        if (Time.time - lastTriggerTime < triggerCooldown) return;
        BugsBunny.LogRed("Bullet hit",this);
        if (other.GetComponent<BaseCharacterController>())
        {
            lastTriggerTime = Time.time;
            GetComponentInParent<BaseCharacterController>().OnBulletHit(other);
        }
    }

    public bool LogThisClass { get; }=true;
}
