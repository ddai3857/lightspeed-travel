using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class player_movement : MonoBehaviour
{
    Rigidbody rb;
    CapsuleCollider col;
    InputAction move;
    InputAction jump;
    Camera cam;
    [SerializeField] float move_speed;
    [SerializeField] float jump_force;
    [SerializeField] float ground_acceleration_rate;
    [SerializeField] float air_acceleration_rate;

    [SerializeField] float ground_dist;
    [SerializeField] LayerMask ground_mask;

    bool jump_pressed = false;
    bool is_grounded = true;
    public static bool freeze = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        move = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        if (jump.WasPressedThisFrame())
        {
            jump_pressed = true;
        }
    }

    void CheckGrounded()
    {
        float ground_radius = col.radius * 0.95f;
        float half_height = col.height * 0.5f;
        is_grounded = Physics.SphereCast(transform.TransformPoint(col.center), ground_radius, Vector3.down, out _, half_height - ground_radius + ground_dist, ground_mask, QueryTriggerInteraction.Ignore);
    }

    void HandleJump()
    {
        if (!is_grounded)
        {
            return;
        }
        rb.AddForce(Vector3.up * jump_force, ForceMode.Impulse);
    }

    void HandleMove()
    {
        Vector2 dir = move.ReadValue<Vector2>();

        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 expected_horizontal_velocity = move_speed * (camForward * dir.y + camRight * dir.x).normalized;
        Vector3 current_horizontal_velocity = new(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float acceleration_rate = is_grounded ? ground_acceleration_rate : air_acceleration_rate;

        Vector3 required_movement_force = Vector3.ClampMagnitude((expected_horizontal_velocity - current_horizontal_velocity) * acceleration_rate, acceleration_rate);
        
        rb.AddForce(required_movement_force, ForceMode.Force);
    }

    void FixedUpdate()
    {
        CheckGrounded();
        if (jump_pressed)
        {
            HandleJump();
            jump_pressed = false;
        }
        HandleMove();
    }
}
