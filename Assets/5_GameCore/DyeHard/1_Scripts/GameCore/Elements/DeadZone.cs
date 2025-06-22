using UnityEngine;
using RedGaint.Games.DyeHard;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BaseCharacterController>())
        {
            other.GetComponent<BaseCharacterController>().KillTheActor();
        }
         
    }
}
