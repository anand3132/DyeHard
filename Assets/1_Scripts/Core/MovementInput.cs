using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour
{
    [Header("Movement Settings")]
    public MovementInputSettings movementSettings;
    private Animator anim;
    private Camera cam;
    private CharacterController controller;
    private bool isGrounded;
    private Vector3 desiredMoveDirection;
    private float inputX;
    private float inputZ;
    private float verticalVel;
    private Vector3 moveVector;

    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    void Update()
    {
        InputMagnitude();
        isGrounded = controller.isGrounded;
        if (isGrounded)
            verticalVel -= 0;
        else
            verticalVel -= 1;

        moveVector = new Vector3(0, verticalVel * .2f * Time.deltaTime, 0);
        controller.Move(moveVector);
    }

    void PlayerMoveAndRotation()
    {
#if UNITY_EDITOR && !ONTEST_INPUT

        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

#else
        inputX = InputHandler.instance.GetRightJoystickDirection.x;
        inputZ = InputHandler.instance.GetRightJoystickDirection.y;
#endif
        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        desiredMoveDirection = forward * inputZ + right * inputX;

        if (!movementSettings.blockRotationPlayer)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), movementSettings.desiredRotationSpeed);
            controller.Move(desiredMoveDirection * Time.deltaTime * movementSettings.velocity);
        }
        else
        {
            controller.Move((transform.forward * inputZ + transform.right * inputX) * Time.deltaTime * movementSettings.velocity);
        }
    }

    void InputMagnitude()
    {
#if UNITY_EDITOR && !ONTEST_INPUT

        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

#else
        inputX = InputHandler.instance.GetRightJoystickDirection.x;
        inputZ = InputHandler.instance.GetRightJoystickDirection.y;
#endif

        float speed = new Vector2(inputX, inputZ).sqrMagnitude;

#if UNITY_EDITOR && !ONTEST_INPUT
        anim.SetBool("OnShooting", Input.GetButton("Fire1"));

#else
        anim.SetBool("OnShooting", InputHandler.instance.GetLeftJoystickDirection.magnitude > 0.1f);

#endif

        if (speed > movementSettings.allowPlayerRotation)
        {
            anim.SetFloat("Blend", speed, movementSettings.startAnimTime, Time.deltaTime);
            anim.SetFloat("X", inputX, movementSettings.startAnimTime / 3, Time.deltaTime);
            anim.SetFloat("Y", inputZ, movementSettings.startAnimTime / 3, Time.deltaTime);
            PlayerMoveAndRotation();
        }
        else if (speed < movementSettings.allowPlayerRotation)
        {
            anim.SetFloat("Blend", speed, movementSettings.stopAnimTime, Time.deltaTime);
            anim.SetFloat("X", inputX, movementSettings.stopAnimTime / 3, Time.deltaTime);
            anim.SetFloat("Y", inputZ, movementSettings.stopAnimTime / 3, Time.deltaTime);
        }
    }
}
