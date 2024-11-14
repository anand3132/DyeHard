using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : Singleton<InputHandler>
{
    [SerializeField] private VariableJoystick rightJoystick;
    [SerializeField] private VariableJoystick leftJoystick;

    // Properties for joystick directions
    public Vector2 GetRightJoystickDirection => rightJoystick?.Direction ?? Vector2.zero;
    public Vector2 GetLeftJoystickDirection => leftJoystick?.Direction ?? Vector2.zero;


    public Dictionary<string, Vector2> GetAllInputs() => new Dictionary<string, Vector2>
    {
        { "RightJoystick", GetRightJoystickDirection },
        { "LeftJoystick", GetLeftJoystickDirection }
    };
}
