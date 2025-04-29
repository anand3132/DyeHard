using UnityEngine;

namespace RedGaint
{
    [CreateAssetMenu(fileName = "MovementInputSettings", menuName = "GameSettings/MovementInputSettings", order = 2)]
    public class MovementInputSettings : ScriptableObject
    {
        [Header("Movement Parameters")] 
        [Range(.1f,20f)]
        public float movementSpeed = 5f;
        public float desiredRotationSpeed = 0.1f;
       // public float speed = 3.5f;
        public float allowPlayerRotation = 0.1f;

        [Header("Animation Smoothing")] [Range(0, 1f)]
        public float startAnimTime = 0.3f;

        [Range(0, 1f)] public float stopAnimTime = 0.15f;



        // public bool blockRotationPlayer { get; internal set; }

        // Add other settings like Input axes or other properties you want to expose
    }
}