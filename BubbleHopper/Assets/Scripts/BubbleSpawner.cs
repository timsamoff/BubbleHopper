using UnityEngine;
using System.Collections;

public class BubbleSpawner : MonoBehaviour
{
    [Header("Bubble Settings")]
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private float minSpawnDistance = 2f;
    [SerializeField] private float maxSpawnDistance = 5f;
    [SerializeField] private float bubbleLifetime = 3f;
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private float popAnimationDuration = 0.5f;
    [SerializeField] private float xAxisThreshold = 3f;
    [SerializeField] private float firstBubbleEdgeOffset = 2f;
    [SerializeField] private float minBubbleSize = 0.5f;
    [SerializeField] private float maxBubbleSize = 1.5f;
    [SerializeField] private float bobbingAmplitude = 0.1f;
    [SerializeField] private float bobbingSpeed = 2f;

    [Header("Water Settings")]
    [SerializeField] private Transform waterSurface;

    [Header("Bubble Testing")]
    [SerializeField] private bool enableForwardMovement = true;
    [SerializeField] private bool enablePopping = true;
    [SerializeField] private bool spawnOneBubbleOnly = false;

    private Transform bubbleParent;
    private GameObject firstBubble;
    private Vector3 lastBubblePosition;
    private bool firstBubblePopped = false;
    private Vector3 waterSize;
    private Vector3 waterPosition;
    private bool placeOnRightSide = true;

    private bool isInputReceived = false; // New flag to track if input was received

    private void Start()
    {
        SetupBubbleParentHierarchy();

        waterSize = waterSurface.localScale;
        waterPosition = waterSurface.position;

        SpawnFirstBubble();
    }

    private void Update()
    {
        // Check for input to start bubble spawning
        if (!isInputReceived && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
                                 Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) ||
                                 Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                                 Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)))
        {
            isInputReceived = true; // Set the flag to true after input is received
            if (!spawnOneBubbleOnly)
            {
                StartCoroutine(SpawnBubbles()); // Start spawning bubbles after input
            }
        }

        if (!firstBubblePopped && firstBubble != null && HasPlayerLeftFirstBubble())
        {
            StartCoroutine(PopBubble(firstBubble));
            firstBubblePopped = true;
            StartCoroutine(SpawnBubbles());
        }
    }

    private void SetupBubbleParentHierarchy()
    {
        GameObject envObject = GameObject.Find("Env");

        if (envObject == null)
        {
            envObject = new GameObject("Env");
        }

        GameObject bubblesObject = envObject.transform.Find("Bubbles")?.gameObject;

        if (bubblesObject == null)
        {
            bubblesObject = new GameObject("Bubbles");
            bubblesObject.transform.SetParent(envObject.transform);
        }

        bubbleParent = bubblesObject.transform;
    }

    private void SpawnFirstBubble()
    {
        Vector3 startPosition = GetFirstBubblePosition();
        firstBubble = Instantiate(bubblePrefab, startPosition, Quaternion.identity, bubbleParent);
        SetRandomBubbleSize(firstBubble);
        lastBubblePosition = startPosition;
        StartCoroutine(RiseBubble(firstBubble));
    }

    private bool HasPlayerLeftFirstBubble()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(player.transform.position, firstBubble.transform.position);
            return distance > 1.5f;
        }
        return false;
    }

    private IEnumerator SpawnBubbles()
    {
        while (isInputReceived) // Continue spawning bubbles if input is received
        {
            SpawnBubble();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnBubble()
    {
        Vector3 spawnPosition = GetValidSpawnPosition();
        GameObject newBubble = Instantiate(bubblePrefab, spawnPosition, Quaternion.identity, bubbleParent);
        SetRandomBubbleSize(newBubble);
        lastBubblePosition = spawnPosition;

        StartCoroutine(RiseBubble(newBubble));

        if (enablePopping)
        {
            StartCoroutine(PopBubble(newBubble));
        }
    }

    private Vector3 GetFirstBubblePosition()
    {
        float halfDepth = waterSize.z * 0.5f;
        float spawnZ = waterPosition.z - halfDepth + firstBubbleEdgeOffset;
        float spawnX = waterPosition.x;  // Ensure X=0 for the first bubble
        float spawnY = waterPosition.y - waterSize.y * 0.5f;  // Fully submerged

        return new Vector3(spawnX, spawnY, spawnZ);
    }

    private Vector3 GetValidSpawnPosition()
    {
        Vector3 spawnPosition;
        int attempts = 0;

        do
        {
            spawnPosition = GetRandomSpawnPosition();
            float distance = Vector3.Distance(spawnPosition, lastBubblePosition);
            if (distance >= minSpawnDistance && distance <= maxSpawnDistance)
            {
                return spawnPosition;
            }
            attempts++;
        }
        while (attempts < 10);

        return lastBubblePosition + new Vector3(0, 0, minSpawnDistance); // Default to min distance if no valid position found
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float halfDepth = waterSize.z * 0.5f;

        float randomZ = enableForwardMovement ? lastBubblePosition.z + Random.Range(minSpawnDistance, maxSpawnDistance) : lastBubblePosition.z;
        float randomX = placeOnRightSide ? waterPosition.x + Random.Range(0, xAxisThreshold) : waterPosition.x - Random.Range(0, xAxisThreshold);
        placeOnRightSide = !placeOnRightSide;
        float spawnY = waterPosition.y - waterSize.y * 0.5f;  // Fully submerged

        return new Vector3(randomX, spawnY, randomZ);
    }

    private IEnumerator RiseBubble(GameObject bubble)
    {
        if (bubble == null) yield break; // Exit if bubble has been destroyed

        Vector3 targetPosition = new Vector3(
            bubble.transform.position.x,
            waterPosition.y + (bubble.transform.localScale.y * 0.5f), // Surface position
            bubble.transform.position.z
        );

        float riseSpeed = 2f;
        while (bubble != null && bubble.transform.position.y < targetPosition.y)
        {
            if (bubble == null) yield break;

            bubble.transform.position = Vector3.MoveTowards(bubble.transform.position, targetPosition, riseSpeed * Time.deltaTime);
            yield return null;
        }

        if (bubble != null)
        {
            StartCoroutine(BobBubble(bubble));
        }
    }

    private IEnumerator BobBubble(GameObject bubble)
    {
        if (bubble == null) yield break;

        float startY = bubble.transform.position.y;

        while (bubble != null)
        {
            float newY = startY + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmplitude;

            if (bubble == null) yield break;

            bubble.transform.position = new Vector3(bubble.transform.position.x, newY, bubble.transform.position.z);
            yield return null;
        }
    }

    private IEnumerator PopBubble(GameObject bubble)
    {
        if (bubble == null || !enablePopping) yield break; // Exit if bubble has been destroyed or popping is disabled

        yield return new WaitForSeconds(bubbleLifetime);

        if (bubble == null) yield break;

        float elapsedTime = 0f;
        Vector3 originalScale = bubble.transform.localScale;

        while (elapsedTime < popAnimationDuration && bubble != null)
        {
            if (bubble == null) yield break;

            bubble.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsedTime / popAnimationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (bubble != null)
        {
            Destroy(bubble);
        }
    }

    private void SetRandomBubbleSize(GameObject bubble)
    {
        float randomSize = Random.Range(minBubbleSize, maxBubbleSize);
        bubble.transform.localScale = new Vector3(randomSize, randomSize, randomSize);
    }
}