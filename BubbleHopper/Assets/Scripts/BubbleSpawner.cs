using UnityEngine;
using System.Collections;

public class BubbleSpawner : MonoBehaviour
{
    [Header("Bubble Spawner Settings")]
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float bubbleSpawnDistance = 100f;
    [SerializeField] private float minZThreshold = 10f;
    [SerializeField] private float maxZThreshold = 15f;
    [SerializeField] private float minXThreshold = -5f;
    [SerializeField] private float maxXThreshold = 5f;
    [SerializeField] private float minBubbleLifetime = 2f;
    [SerializeField] private float maxBubbleLifetime = 5f;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float popThreshold = 2f;

    [Header("Water Surface")]
    [SerializeField] private Transform waterSurface;

    [Header("Bubble Movement Settings")]
    [SerializeField] private bool bubbleMovementEnabled = true;
    [SerializeField] private bool bubblePoppingEnabled = true;

    [Header("Player Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float zSubtractedFromPlayer = 3f;

    private GameObject firstBubble;
    private Vector3 lastBubblePosition;
    private bool firstBubblePopped = false;
    private Vector3 waterSize;
    private Vector3 waterPosition;
    private bool placeOnRightSide = true;

    private void Start()
    {
        waterSize = waterSurface.localScale;
        waterPosition = waterSurface.position;
        SpawnFirstBubble();
    }

    private void Update()
    {
        if (!firstBubblePopped && firstBubble != null && HasPlayerLeftFirstBubble())
        {
            StartCoroutine(PopBubble(firstBubble));
            firstBubblePopped = true;
            StartCoroutine(SpawnBubbles());
        }
    }

    private void SpawnFirstBubble()
    {
        Vector3 startPosition = GetFirstBubblePosition();
        firstBubble = Instantiate(bubblePrefab, startPosition, Quaternion.identity);
        SetRandomBubbleSize(firstBubble);
        lastBubblePosition = startPosition;
        StartCoroutine(RiseBubble(firstBubble));
    }

    private bool HasPlayerLeftFirstBubble()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(player.position, firstBubble.transform.position);
            return distance > 1.5f;
        }
        return false;
    }

    private IEnumerator SpawnBubbles()
    {
        while (true)
        {
            SpawnBubble();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnBubble()
    {
        Vector3 spawnPosition = GetValidSpawnPosition();
        GameObject newBubble = Instantiate(bubblePrefab, spawnPosition, Quaternion.identity);
        SetRandomBubbleSize(newBubble);
        lastBubblePosition = spawnPosition;

        StartCoroutine(RiseBubble(newBubble));

        if (bubbleMovementEnabled)
        {
            StartCoroutine(FloatBubble(newBubble));  // Start floating towards the player
        }

        if (bubblePoppingEnabled)
        {
            StartCoroutine(PopBubble(newBubble)); // Pop bubble after its lifetime
        }
    }

    private Vector3 GetFirstBubblePosition()
    {
        if (player != null)
        {
            Vector3 playerPosition = player.position;

            return new Vector3(playerPosition.x, waterPosition.y - waterSize.y * 0.5f, playerPosition.z);
        }
        return Vector3.zero;
    }

    private Vector3 GetValidSpawnPosition()
    {
        float spawnZ = player.position.z + bubbleSpawnDistance;

        spawnZ += Random.Range(minZThreshold, maxZThreshold);

        float spawnX = player.position.x + Random.Range(minXThreshold, maxXThreshold);

        float spawnY = waterPosition.y - waterSize.y * 0.5f;

        return new Vector3(spawnX, spawnY, spawnZ);
    }

    private IEnumerator RiseBubble(GameObject bubble)
    {
        Vector3 targetPosition = new Vector3(
            bubble.transform.position.x,
            waterPosition.y + (bubble.transform.localScale.y * 0.5f), // Surface position
            bubble.transform.position.z
        );

        float riseSpeed = 2f;
        while (bubble != null && bubble.transform.position.y < targetPosition.y)
        {
            bubble.transform.position = Vector3.MoveTowards(bubble.transform.position, targetPosition, riseSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator FloatBubble(GameObject bubble)
    {
        if (bubble == null) yield break;

        Vector3 startPosition = bubble.transform.position;
        Vector3 targetPosition = new Vector3(
            startPosition.x,
            startPosition.y,
            player.position.z - zSubtractedFromPlayer  // Move towards the player along the Z-axis
        );

        float timeElapsed = 0f;
        while (bubble != null && timeElapsed < floatSpeed)
        {
            bubble.transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / floatSpeed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        bubble.transform.position = targetPosition; // Ensure it reaches the final position
    }

    private IEnumerator PopBubble(GameObject bubble)
    {
        if (bubble == null || !bubblePoppingEnabled || firstBubble == bubble) yield break; // Exit if bubble is first or popping is disabled

        float randomLifetime = Random.Range(minBubbleLifetime, maxBubbleLifetime);

        yield return new WaitForSeconds(randomLifetime); // Wait for random lifetime

        float elapsedTime = 0f;
        Vector3 originalScale = bubble.transform.localScale;

        while (elapsedTime < popThreshold && bubble != null)
        {
            bubble.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsedTime / popThreshold);
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
        float randomSize = Random.Range(0.5f, 1.5f);
        bubble.transform.localScale = new Vector3(randomSize, randomSize, randomSize);
    }
}

