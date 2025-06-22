using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    [CreateAssetMenu(fileName = "BotSettings", menuName = "GameSettings/BotSettings", order = 3)]
    public class BotSettings : ScriptableObject
    {
        [Header("Detection Settings")]//----------------------------------------------------
        [Tooltip("How far the bot can see.")]
        [Range(0, 80)] [SerializeField] public float sightRange = 20f;

        [Tooltip("Field of view angle for the bot.")]
        [Range(15, 180)] [SerializeField] public float fovAngle = 45f;
        
        [Header("Attack Settings")]//---------------------------------------------------
        [Tooltip("The range at which the bot can attack.")]
        [Range(3, 10)] [SerializeField] public float attackRange = 3f;

        [Tooltip("distance at which bot start attack.")]

        [Range(0, 30)] [SerializeField] public float attackFromDistance = 3f;
        
        [Header("Movement Settings")]//---------------------------------------------------

        [Tooltip("The speed at which the bot moves.")]
        [Range(.1f, 15)] [SerializeField] public float movementSpeed = 3f;

        [Tooltip("The maximum distance the bot will follow a target.")]
        [Range(3, 30)] [SerializeField] public int maxFollowRange = 3;


        [Header("Rotation Settings")]//-----------------------------------------------------
        [Tooltip("Minimum random angle for left rotation.")]
        [Range(0, 360f)] [SerializeField] public float minRotationAngle = -15f;

        [Tooltip("Maximum random angle for right rotation.")]
        [Range(0, 360f)] [SerializeField] public float maxRotationAngle = 15f;

        [Tooltip("Duration to complete the rotation.")]
        
        [SerializeField] [Range(0, 20f)] public float rotationDuration = 2f;
        
        [Header("Animation Smoothing")]//---------------------------------------------------

        [Tooltip("Time delay to start the walk animation.")]
        [Range(0, 1f)]  [SerializeField] public float startAnimTime = 0.3f;

        [Tooltip("Time delay to stop the walk animation.")]
        [Range(0, 1f)]  [SerializeField] public float stopAnimTime = 0.15f;
        
        [Tooltip("Time delay to stop the walk animation.")]
        [Range(.01f, 1.0f)]  [SerializeField] public float animationSpeedOffset = 0.5f; 

    }
}
