using UnityEngine;

public class MoveEnvironment : MonoBehaviour
{
    [SerializeField] private float moveDistance = 450f;
    [SerializeField] private float moveSpeed = .025f;
    [SerializeField] private float easeInDuration = 0.5f;
    [SerializeField] private float easeOutDuration = 0.5f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isEaseOut = false;
    private float elapsedTime = 0f;
    private float steadySpeedDuration;
    private float totalDuration;

    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition - new Vector3(0, 0, moveDistance);

        steadySpeedDuration = moveDistance / moveSpeed - (easeInDuration + easeOutDuration);

        if (steadySpeedDuration < 0)
        {
            steadySpeedDuration = 0;
            Debug.LogWarning("Ease in/out durations exceed total movement time. Adjust values.");
        }

        totalDuration = easeInDuration + steadySpeedDuration + easeOutDuration;
    }

    void Update()
    {
        if (!isMoving && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)
                         || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            isMoving = true;
            elapsedTime = 0f;
        }

        if (isMoving)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsedTime / totalDuration);

            float easedProgress = EaseInOut(progress);

            transform.position = Vector3.Lerp(startPosition, targetPosition, easedProgress);

            if (elapsedTime >= totalDuration)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }

        if (isEaseOut)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsedTime / easeOutDuration);
            float easedProgress = Mathf.Pow(1f - progress, 2);
            transform.position = Vector3.Lerp(transform.position, targetPosition, easedProgress);

            if (progress >= 1f)
            {
                isMoving = false;
            }
        }
    }

    private float EaseInOut(float t)
    {
        if (t < easeInDuration / totalDuration)
        {
            float easeInTime = t / (easeInDuration / totalDuration);
            return Mathf.Pow(easeInTime, 2) * (moveDistance / totalDuration);
        }
        else if (t > 1f - (easeOutDuration / totalDuration))
        {
            float easeOutTime = (t - (1f - (easeOutDuration / totalDuration))) / (easeOutDuration / totalDuration);
            return moveDistance - Mathf.Pow(1f - easeOutTime, 2) * (moveDistance / totalDuration);
        }
        else
        {
            float linearTime = (t - (easeInDuration / totalDuration)) / (steadySpeedDuration / totalDuration);
            return (easeInDuration / totalDuration) * moveDistance + linearTime * (moveDistance - (easeInDuration + easeOutDuration) / totalDuration * moveDistance);
        }
    }

    public void StartEaseOut()
    {
        isEaseOut = true;
        elapsedTime = 0f;
    }
}
