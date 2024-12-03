using System;
using System.Collections;
using System.Collections.Generic;
using RedGaint;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BaseCharacterController>())
        {
            GetComponentInParent<BaseCharacterController>().OnBulletHit(other);
        }
    
    }
}
