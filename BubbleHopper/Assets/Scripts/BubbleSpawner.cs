using UnityEngine;
using System.Collections;

public class BubbleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private float minSpawnDistance = 2f;
    [SerializeField] private float maxSpawnDistance = 5f;
    [SerializeField] private float bubbleLifetime = 3f;
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private float popAnimationDuration = 0.5f;
    [SerializeField] private Transform waterSurface;
    [SerializeField] private float zAxisThreshold = 3f;  // Bubble stray range on Z-axis
    [SerializeField] private float firstBubbleEdgeOffset = 2f;  // Distance from the -X edge
    [SerializeField] private float minBubbleSize = 0.5f;  // Min bubble size
    [SerializeField] private float maxBubbleSize = 1.5f;  // Max bubble size

    private GameObject firstBubble;
    private Vector3 lastBubblePosition;
    private bool firstBubblePopped = false;
    private Vector3 waterSize;
    private Vector3 waterPosition;

    private void Start()
    {
        waterSize = waterSurface.localScale;
        waterPosition = waterSurface.position;
        SpawnFirstBubble();
    }

    private void SpawnFirstBubble()
    {
        Vector3 startPosition = GetFirstBubblePosition();
        firstBubble = Instantiate(bubblePrefab, startPosition, Quaternion.identity);
        SetRandomBubbleSize(firstBubble);
        lastBubblePosition = startPosition;
        StartCoroutine(RiseBubble(firstBubble));
    }

    private void Update()
    {
        if (!firstBubblePopped && HasPlayerLeftFirstBubble())
        {
            StartCoroutine(PopBubble(firstBubble));
            firstBubblePopped = true;
            StartCoroutine(SpawnBubbles());
        }
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
        StartCoroutine(PopBubble(newBubble));
    }

    private Vector3 GetFirstBubblePosition()
    {
        float halfWidth = waterSize.x * 0.5f;
        float spawnX = waterPosition.x - halfWidth + firstBubbleEdgeOffset;
        float spawnZ = waterPosition.z;  // Ensure Z=0 for the first bubble
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

        return lastBubblePosition + new Vector3(minSpawnDistance, 0, 0); // Default to min distance if no valid position found
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float halfWidth = waterSize.x * 0.5f;

        float randomX = lastBubblePosition.x + Random.Range(minSpawnDistance, maxSpawnDistance);
        float randomZ = waterPosition.z + Random.Range(-zAxisThreshold, zAxisThreshold);
        float spawnY = waterPosition.y - waterSize.y * 0.5f;  // Fully submerged

        return new Vector3(randomX, spawnY, randomZ);
    }

    private IEnumerator RiseBubble(GameObject bubble)
    {
        Vector3 targetPosition = new Vector3(
            bubble.transform.position.x,
            waterPosition.y + (bubble.transform.localScale.y * 0.5f), // Surface position
            bubble.transform.position.z
        );

        float riseSpeed = 2f;
        while (bubble.transform.position.y < targetPosition.y)
        {
            bubble.transform.position = Vector3.MoveTowards(bubble.transform.position, targetPosition, riseSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator PopBubble(GameObject bubble)
    {
        yield return new WaitForSeconds(bubbleLifetime);

        float elapsedTime = 0f;
        Vector3 originalScale = bubble.transform.localScale;

        while (elapsedTime < popAnimationDuration)
        {
            bubble.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsedTime / popAnimationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(bubble);
    }

    private void SetRandomBubbleSize(GameObject bubble)
    {
        float randomSize = Random.Range(minBubbleSize, maxBubbleSize);
        bubble.transform.localScale = new Vector3(randomSize, randomSize, randomSize);
    }
}