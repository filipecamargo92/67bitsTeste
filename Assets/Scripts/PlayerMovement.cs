using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    PlayerInput input;
    Animator anim;

    Vector2 moveInput;
    Vector3 direction;
    Vector3 currentVelocity;
    float rotationSmoothVel;

    #region INSPECTOR
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float smoothVelocity = 0.2f;
    [SerializeField] float bodyRotationSmooth = 0.1f;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        if (input == null) input = new PlayerInput();
        input.Player.Enable();

        input.Player.Movement.performed += MovementPerformed;
        input.Player.Movement.canceled += MovementCanceled;
    }

    private void OnDisable()
    {
        input.Player.Movement.canceled -= MovementCanceled;
        input.Player.Movement.performed -= MovementPerformed;

        input.Player.Disable();
    }

    private void Update()
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSmoothVel, bodyRotationSmooth);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    private void FixedUpdate()
    {
        Vector3 targetVelocity = new Vector3(moveInput.x, 0f, moveInput.y).normalized * moveSpeed;
        Vector3 smoothedVelocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, smoothVelocity);
        rb.velocity = new Vector3(smoothedVelocity.x, rb.velocity.y, smoothedVelocity.z);
    }

    private void MovementPerformed(InputAction.CallbackContext ctx)
    { 
        moveInput = ctx.ReadValue<Vector2>();
        direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        anim.SetBool("Moving", true);
    }

    private void MovementCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
        anim.SetBool("Moving", false);
    }
}