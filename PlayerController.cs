using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Lane Configuration")]
    public Transform[] lanes;
    private int currentLane = 2;
    public float moveSpeed = 5f;

    [Header("Movement Flags")]
    private bool isMoving = false;
    private Vector3 targetPosition;

    void Start()
    {
        if (lanes.Length == 0)
        {
            Debug.LogError("No lanes assigned to PlayerController.");
            return;
        }

        transform.position = new Vector3(lanes[currentLane].position.x, transform.position.y, transform.position.z);
    }

    void Update()
    {
        HandleInput();

        if (isMoving)
        {
            MoveTowardsTarget();
        }
    }

    void HandleInput()
    {
        // Move left
        if (Input.GetKeyDown(KeyCode.LeftArrow) && currentLane > 0 && !isMoving)
        {
            currentLane--;
            SetTargetPosition();
        }
        // Move right
        else if (Input.GetKeyDown(KeyCode.RightArrow) && currentLane < lanes.Length - 1 && !isMoving)
        {
            currentLane++;
            SetTargetPosition();
        }

        // Check for spacebar press to trigger note hit
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckForNoteHit();
        }
    }

    void SetTargetPosition()
    {
        float targetX = lanes[currentLane].position.x;
        targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
        isMoving = true;
    }

    void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
        {
            isMoving = false;
        }
    }

    void CheckForNoteHit()
    {
        float hitRadius = 0.5f;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, hitRadius);

        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Note"))
            {
                collider.GetComponent<NoteController>().OnHit();
            }
        }
    }
}
