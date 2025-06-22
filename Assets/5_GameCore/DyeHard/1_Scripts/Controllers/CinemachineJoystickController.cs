using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
 
namespace RedGaint.Games.DyeHard
{
    public class CinemachineFreeLookController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CinemachineFreeLook freeLookCamera; // Reference to the Cinemachine FreeLook camera
        [SerializeField] private InputActionReference lookAction;    // Reference to the "Look" action from the Input Actions asset

        [Header("Sensitivity")]
        [SerializeField] private float sensitivityX = 2f;            // Horizontal sensitivity
        [SerializeField] private float sensitivityY = 2f;            // Vertical sensitivity

        private void Awake()
        {
            if (freeLookCamera == null)
            {
                Debug.LogError("Cinemachine FreeLook Camera is not assigned.");
                enabled = false;
                return;
            }

            if (lookAction == null)
            {
                Debug.LogError("Look InputAction is not assigned.");
                enabled = false;
                return;
            }
        }

        private void OnEnable()
        {
            lookAction.action.Enable();
        }

        private void OnDisable()
        {
            lookAction.action.Disable();
        }

        private void Update()
        {
            if (freeLookCamera == null || lookAction == null) return;

            // Get the Look input
            Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

            // Apply input to the FreeLook camera's axes
            freeLookCamera.m_XAxis.Value += lookInput.x * sensitivityX;      // Horizontal rotation
            freeLookCamera.m_YAxis.Value -= lookInput.y * sensitivityY;      // Vertical rotation
        }
    }
}