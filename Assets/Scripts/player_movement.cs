using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class player_movement : MonoBehaviour
{
    Rigidbody rb;
    InputAction move;
    Camera cam;
    [SerializeField] float move_speed = 4f;

    public static bool freeze = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        move = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Vector2 dir = move.ReadValue<Vector2>();

        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = move_speed * Time.deltaTime * ((camForward * dir.y) + (camRight * dir.x));

        rb.AddForce(moveDirection, ForceMode.Force);
            // rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);


        // Clamp velocity instead of always setting it so that drag can slow us down instead
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > move_speed)
        {
            Vector3 limitedVel = flatVel.normalized * move_speed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }
}
