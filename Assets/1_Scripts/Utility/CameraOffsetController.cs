using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedGaint
{
    public class CameraOffsetController : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float rotationSpeed = 5f;
        public  VariableJoystick Joystick;

        private float yawOffset;
        private float pitchOffset;

        private void Update()
        {
            AdjustCameraRotation();
        }

        private void AdjustCameraRotation()
        {
            Vector2 joystickInput = Joystick.Direction;
            // Check if there is any input
            if (joystickInput.magnitude > 0.1f)
            {
                // Calculate the desired rotation angle based on joystick input
                float desiredYaw = joystickInput.x * rotationSpeed * Time.deltaTime;
                float desiredPitch = joystickInput.y * rotationSpeed * Time.deltaTime;

                // Apply rotation offsets
                yawOffset += desiredYaw;
                pitchOffset += desiredPitch;

                cameraTransform.Rotate(Vector3.up, desiredYaw, Space.World);
                cameraTransform.Rotate(Vector3.right, -desiredPitch, Space.Self);
            }
        }

        private void OnGUI()
        {
            // Create a custom GUIStyle with a larger font size
            GUIStyle largeTextStyle = new GUIStyle(GUI.skin.label);
            largeTextStyle.fontSize = 20;  // Set font size here
            largeTextStyle.normal.textColor = Color.red;  // Set text color if desired

            // Display the yaw and pitch offset values in the top-left corner with the custom style
            GUI.Label(new Rect(10, 10, 300, 30), $"Yaw Offset: {yawOffset:F2}", largeTextStyle);
            GUI.Label(new Rect(10, 40, 300, 30), $"Pitch Offset: {pitchOffset:F2}", largeTextStyle);
        }
    }
}