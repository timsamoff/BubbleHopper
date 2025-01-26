using UnityEngine;
using Unity.Collections;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private MoveEnvironment moveEnvironment;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("FinishLine"))
        {
            Invoke("EndGame", 1f);
        }
    }

    void EndGame()
    {
        moveEnvironment.enabled = false;
        SceneManager.LoadScene("End");
    }
}