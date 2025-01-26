using UnityEngine;

public class BugController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float zRotationAmount = 20f;
    [SerializeField] private float yRotationAmount = 10f;
    [SerializeField] private float rotationSpeed = 5f;

    private float moveInput;
    private Quaternion originalRotation;
    private Vector3 initialPosition;

    void Start()
    {
        originalRotation = transform.rotation;
        initialPosition = transform.position;
    }

    void Update()
    {
        moveInput = -Input.GetAxis("Horizontal");

        Vector3 movement = new Vector3(moveInput * moveSpeed * Time.deltaTime, 0, 0);
        transform.position += transform.TransformDirection(movement);
        transform.position = new Vector3(transform.position.x, initialPosition.y, initialPosition.z);

        float targetRotationZ = moveInput * zRotationAmount;
        float targetRotationY = moveInput * yRotationAmount;

        Quaternion targetRotation = originalRotation * Quaternion.Euler(0, targetRotationY, targetRotationZ);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}