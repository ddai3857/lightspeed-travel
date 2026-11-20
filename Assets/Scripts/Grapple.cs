using UnityEngine;
using UnityEngine.InputSystem;

public class Grapple : MonoBehaviour
{
    Camera cameraChild;
    Rigidbody rigBod;
    // Grapple things
    [SerializeField] GameObject grapplingHook;
    [SerializeField] LineRenderer grappleLine;
    [SerializeField] float maxGrappleDist = 50f;

    Vector3 grappleTargetPoint = Vector3.zero;
    Vector3 grappleStartPoint = Vector3.zero;
    float grappleDistance = 0; // Target - Start for easier math
    float grappleLerp = 0f; // Goes from 0 -> 1 per delta time
    [SerializeField] float grapplePullSpeed = 10f;
    [SerializeField] float grappleSpeed = 100;
    bool grappleHit = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputAction grapple_pressed = InputSystem.actions.FindAction("Attack");

        cameraChild = GetComponentInChildren<Camera>();
        rigBod = GetComponent<Rigidbody>();

        grapple_pressed.started += GrappleStart;
        grapple_pressed.canceled += GrappleFinished;
        grapple_pressed.Enable();

        grappleLine.enabled = false;
    }

    void OnDestroy()
    {
        InputAction grapple_pressed = InputSystem.actions.FindAction("Attack");

        grapple_pressed.started -= GrappleStart;
        grapple_pressed.canceled -= GrappleFinished;
    }

    // Mouse down
    void GrappleStart(InputAction.CallbackContext context)
    {
        Debug.Log("Start grapple");
        RaycastHit hit;
        grappleStartPoint = grapplingHook.transform.position;

        // Can add a float maxdist and int layerMask for grapplable things
        if (Physics.Raycast(cameraChild.transform.position, cameraChild.transform.forward, out hit, maxGrappleDist))
        {
            grappleTargetPoint = hit.point;
            Debug.Log("Hit " + grappleTargetPoint);
            grappleLine.enabled = true;
            grappleHit = true;
        }
        else
        {
            grappleTargetPoint = cameraChild.transform.position + cameraChild.transform.forward * maxGrappleDist;
            Debug.Log("Hit " + grappleTargetPoint);
            grappleLine.enabled = true;   
            grappleHit = false;
        }
        grappleDistance = (grappleTargetPoint-grappleStartPoint).magnitude;
    }

    // Mouse release
    void GrappleFinished(InputAction.CallbackContext context)
    {
        Debug.Log("Finish grapple");
        ResetGrapple();
    }

    void ResetGrapple()
    {
        grappleLine.enabled = false;
        grappleHit = false;
        grappleLerp = 0f;
    }

    void GrapplePull()
    {
        Vector3 direction = grappleLine.GetPosition(1) - grappleLine.GetPosition(0);
        Vector3 addedLinvelocity = direction.normalized * grapplePullSpeed;
        rigBod.AddForce(addedLinvelocity, ForceMode.Force);
    }


    // Update is called once per frame
    void LateUpdate()
    {
        if (grappleLine.enabled)
        {
            if (grappleLerp < 1)
            {
                grappleLine.SetPosition(1, Vector3.Lerp(grappleStartPoint, grappleTargetPoint, grappleLerp));
                grappleLerp += Time.deltaTime * grappleSpeed / grappleDistance;
            }
            else
            {
                // Got to target
                grappleLine.SetPosition(1, grappleTargetPoint);

                // If didn't actually hit an obstacle, reset the grapple.
                if (!grappleHit)
                {
                    ResetGrapple();
                }

                GrapplePull();
            }

            // Also move start to where player is now
            grappleLine.SetPosition(0, grapplingHook.transform.position);
        }
    }
}
