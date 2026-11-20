using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    CapsuleCollider col;
    Camera cam;
    InputAction move;
    InputAction jump;
    InputAction dash;

    [Header("Basic Movement")]
    [SerializeField] float move_speed;
    [SerializeField] float jump_force;
    [SerializeField] float ground_acceleration_rate;
    [SerializeField] float air_acceleration_rate;

    [Header("Ground Checking")]

    [SerializeField] float ground_dist;
    [SerializeField] LayerMask ground_mask;

    [Header("Dash")]
    [SerializeField] float dash_duration;
    [SerializeField] float dash_speed;
    [SerializeField] float dash_end_speed;

    [SerializeField] float dash_cooldown;

    bool jump_pressed = false;
    bool is_grounded = true;

    bool can_dash = true;
    bool is_dashing = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        move = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");
        dash = InputSystem.actions.FindAction("Dash");
    }

    // Update is called once per frame
    void Update()
    {
        if (jump.WasPressedThisFrame())
        {
            jump_pressed = true;
        }

        if (dash.WasPressedThisFrame() && can_dash)
        {
            StartCoroutine(Dash());
        }

        Debug.Log("ISGROUNED: " + is_grounded);
        Debug.Log("CANDASH: " + can_dash);

    }

    void CheckGrounded()
    {
        float ground_radius = col.radius * 0.95f;
        float half_height = col.height * 0.5f;
        is_grounded = Physics.SphereCast(transform.TransformPoint(col.center), ground_radius, Vector3.down, out _, half_height - ground_radius + ground_dist, ground_mask, QueryTriggerInteraction.Ignore);
    }

    public bool IsGrounded()
    {
        return is_grounded;
    }

    void HandleJump()
    {
        if (!is_grounded)
        {
            return;
        }
        rb.AddForce(Vector3.up * jump_force, ForceMode.Impulse);
    }

    Vector3 GetMoveDirection(bool for_dash = false)
    {
        Vector2 dir = move.ReadValue<Vector2>();

        if (dir == Vector2.zero && for_dash)
        {
            dir = Vector2.up;
        }

        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        return (camForward * dir.y + camRight * dir.x).normalized;
    }

    void HandleMove()
    {
        Vector3 expected_horizontal_velocity = move_speed * GetMoveDirection();
        Vector3 current_horizontal_velocity = new(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float acceleration_rate = is_grounded ? ground_acceleration_rate : air_acceleration_rate;

        Vector3 required_movement_force = Vector3.ClampMagnitude((expected_horizontal_velocity - current_horizontal_velocity) * acceleration_rate, acceleration_rate);
        
        rb.AddForce(required_movement_force, ForceMode.Force);
    }

    void FixedUpdate()
    {
        CheckGrounded();
        if (is_dashing)
        {
            return;
        }

        if (jump_pressed)
        {
            HandleJump();
            jump_pressed = false;
        }
        HandleMove();
    }

    IEnumerator Dash()
    {
        can_dash = false;
        is_dashing = true;
        bool prev_gravity = rb.useGravity;
        rb.useGravity = false;

        rb.linearVelocity = GetMoveDirection(true) * dash_speed;
        yield return new WaitForSeconds(dash_duration);
        
        rb.linearVelocity = rb.linearVelocity.normalized * dash_end_speed;
        rb.useGravity = prev_gravity;
        is_dashing = false;

        yield return new WaitForSeconds(dash_cooldown);
        yield return new WaitUntil(() => is_grounded);
        can_dash = true;
    }
}
