using UnityEngine;
using UnityEngine.InputSystem;

public class Grapple : MonoBehaviour
{
    Camera cameraChild;

    // Grapple things
    [SerializeField] GameObject grapplingHook;
    [SerializeField] LineRenderer grappleLine;
    float maxGrappleDist = 50f;

    Vector3 grappleTargetPoint = Vector3.zero;
    Vector3 grappleStartPoint = Vector3.zero;
    float grappleTravelTime = 0.2f;
    float grappleCurrentTime = 0f;


    bool grappleHit = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputAction grapple_pressed = InputSystem.actions.FindAction("Attack");

        cameraChild = GetComponentInChildren<Camera>();

        grapple_pressed.started += GrappleStart;
        grapple_pressed.canceled += GrappleFinished;
        grapple_pressed.Enable();

        grappleLine.enabled = false;
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
        grappleCurrentTime = 0f;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (grappleLine.enabled)
        {
            if (grappleCurrentTime < grappleTravelTime)
            {
                grappleLine.SetPosition(1, Vector3.Lerp(grappleStartPoint, grappleTargetPoint, grappleCurrentTime/grappleTravelTime));
                Debug.Log(grappleCurrentTime/grappleTravelTime);
                grappleCurrentTime += Time.deltaTime;
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
            }

            // Also move start to where player is now
            grappleLine.SetPosition(0, grapplingHook.transform.position);
        }
    }
}
