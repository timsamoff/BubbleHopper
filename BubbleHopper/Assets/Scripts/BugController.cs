using UnityEngine;

public class BugController : MonoBehaviour
{
    [Header("Bug Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float zRotationAmount = 20f;
    [SerializeField] private float yRotationAmount = 10f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float blockDelay = 0.25f;

    private float moveInput;
    private Quaternion originalRotation;
    private Vector3 initialPosition;
    private bool isBlockedByBoundary = false;
    private float blockTimer = 0f;

    void Start()
    {
        originalRotation = transform.rotation;
        initialPosition = transform.position;
    }

    void Update()
    {
        if (isBlockedByBoundary)
        {
            blockTimer += Time.deltaTime;
            if (blockTimer >= blockDelay)
            {
                isBlockedByBoundary = false;
                blockTimer = 0f;
            }
            return;
        }

        moveInput = -Input.GetAxis("Horizontal");

        Vector3 movement = new Vector3(moveInput * moveSpeed * Time.deltaTime, 0, 0);
        transform.position += transform.TransformDirection(movement);
        transform.position = new Vector3(transform.position.x, initialPosition.y, initialPosition.z);

        float targetRotationZ = moveInput * zRotationAmount;
        float targetRotationY = moveInput * yRotationAmount;

        Quaternion targetRotation = originalRotation * Quaternion.Euler(0, targetRotationY, targetRotationZ);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boundaries"))
        {
            isBlockedByBoundary = true;
            blockTimer = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boundaries"))
        {
            blockTimer = 0f;
            isBlockedByBoundary = true;
        }
    }
}