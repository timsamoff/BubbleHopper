using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private MoveEnvironment moveEnvironment;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FinishLine"))
        {
            if (moveEnvironment != null)
            {
                moveEnvironment.StartEaseOut();
            }
        }
    }
}