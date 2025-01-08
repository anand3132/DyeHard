using System;
using System.Collections;
using System.Collections.Generic;
using RedGaint;
using UnityEngine;


public class Bullet : MonoBehaviour
{
    private float lastTriggerTime = 0f; 
    private const float triggerCooldown = 1f;
    
    private void OnTriggerStay(Collider other)
    {
        if (Time.time - lastTriggerTime < triggerCooldown) return;
        
        if (other.GetComponent<BaseCharacterController>())
        {
            lastTriggerTime = Time.time;
            GetComponentInParent<BaseCharacterController>().OnBulletHit(other);
        }
    }
}
