using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Rigidbody rb;
    private Camera cam;
    private Worker worker;

    private bool isDragging = false;
    private Plane dragPlane;

    private ActivityZone currentZone;

    public bool IsDragging => isDragging;
    public LayerMask floorLayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        worker = GetComponent<Worker>();

        dragPlane = new Plane(Vector3.up, Vector3.zero);
    }

    void OnMouseDown()
    {
        isDragging = true;

        // Prevent the object from fighting against dragging
        rb.useGravity = false;
    }


    void OnMouseUp()
    {
        isDragging = false;

        // Gravity takes control again
        rb.useGravity = true;

        if (currentZone != null)
        {
            Debug.Log(gameObject.name + " dropped in: " + currentZone.zoneType);
            worker.SetActivity(currentZone.zoneType);
        }
        else
        {
            Debug.Log(gameObject.name + " dropped in no activity zone.");
            worker.SetIdle();
        }
    }


    void FixedUpdate()
    {
        if (!isDragging)    
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);

            Vector3 targetPosition = new Vector3(
                point.x,
                rb.position.y,
                point.z
            );
            
            Vector3 movement = targetPosition - rb.position;
            float moveDistance = movement.magnitude;

            if(moveDistance > 0.01f)
            {
                Vector3 direction = movement.normalized;

                if(rb.SweepTest(
                    direction, 
                    out RaycastHit hit, 
                    moveDistance,
                    QueryTriggerInteraction.Ignore))
                {
                    float safeDistance = Mathf.Max(0f, hit.distance - 0.02f);

                    rb.MovePosition(rb.position + direction * safeDistance);
                }
                else
                {
                    // No collision, move to the target position
                    rb.MovePosition(targetPosition);
                }
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        ActivityZone zone = other.GetComponent<ActivityZone>();
        if (zone != null)
        {
            currentZone = zone;

            Debug.Log(gameObject.name + " entered " + zone.zoneType);
        }
    }


    void OnTriggerExit(Collider other)
    {
        ActivityZone zone = other.GetComponent<ActivityZone>();
        if (zone != null && zone == currentZone)
        {
            Debug.Log(gameObject.name + " exited " + zone.zoneType);

            currentZone = null;
        }
    }
}
