using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private float xAngle = 30f;
    [SerializeField] private float yAngle = 0f;

    private void LateUpdate()
    {
        if (player == null)
            return;

        Vector3 desiredPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(xAngle, yAngle, 0f);
    }
}