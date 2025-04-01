using UnityEngine;

public class PlatformShopPoint : MonoBehaviour
{
    [Header("Налаштування")]
    public float cost = 0f;
    public GameObject platformPrefab;
    public GameObject[] wallPrefabs; // Масив варіантів стін
    public float wallCheckDistance = 1.5f;

    [Header("Візуальні ефекти")]
    public GameObject purchasePrompt;
    public ParticleSystem purchaseEffect;
    public AudioClip purchaseSound;
    public float destroyDelay = 0.5f;

    private bool isPurchased = false;
    private bool playerInRange = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (purchasePrompt != null) purchasePrompt.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isPurchased || !other.CompareTag("Player")) return;
        playerInRange = true;
        if (purchasePrompt != null) purchasePrompt.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (purchasePrompt != null) purchasePrompt.SetActive(false);
    }

    void Update()
    {
        if (!isPurchased && playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            AttemptPurchase();
        }
    }

    void AttemptPurchase()
    {
        var player = FindObjectOfType<main_player_script>();
        if (player == null || player.PlayerMoney < cost) return;
        CompletePurchase(player);
    }

    void CompletePurchase(main_player_script player)
    {
        isPurchased = true;
        player.PlayerMoney -= cost;

        Vector3 spawnPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Instantiate(platformPrefab, spawnPosition, Quaternion.identity);

        SpawnWalls(spawnPosition);
        PlayPurchaseEffects();
        DisableShopPoint();
        Destroy(gameObject, destroyDelay);
    }

    void SpawnWalls(Vector3 platformPosition)
    {
        // Якщо немає префабів, використовуємо перший (для зворотної сумісності)
        GameObject wallToSpawn = wallPrefabs != null && wallPrefabs.Length > 0 ?
            wallPrefabs[Random.Range(0, wallPrefabs.Length)] :
            wallPrefabs[0];

        Vector3 platformSize = platformPrefab.transform.localScale;
        Vector3 wallSize = wallToSpawn.transform.localScale;

        Vector3[] directions = { Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        foreach (Vector3 dir in directions)
        {
            CheckAndSpawnWall(platformPosition, dir, platformSize, wallSize, wallToSpawn);
        }
    }

    void CheckAndSpawnWall(Vector3 platformPosition, Vector3 direction, Vector3 platformSize, Vector3 wallSize, GameObject wallPrefab)
    {
        float rayLength = direction.x != 0 ? platformSize.x / 2 + wallCheckDistance : platformSize.z / 2 + wallCheckDistance;
        Vector3 rayOrigin = platformPosition;
        rayOrigin.y = 0;

        if (!Physics.Raycast(rayOrigin, direction, rayLength))
        {
            Vector3 wallPosition = platformPosition;

            if (direction.x != 0)
            {
                wallPosition.x += direction.x * (platformSize.x / 2 + wallSize.x / 2);
                wallPosition.y = wallSize.y / 2;
            }
            else
            {
                wallPosition.z += direction.z * (platformSize.z / 2 + wallSize.z / 2);
                wallPosition.y = wallSize.y / 2;
            }

            Quaternion wallRotation = Quaternion.LookRotation(direction);
            Instantiate(wallPrefab, wallPosition, wallRotation);
        }
    }

    void PlayPurchaseEffects()
    {
        if (purchaseEffect != null)
            Instantiate(purchaseEffect, transform.position, Quaternion.identity);

        if (purchaseSound != null)
        {
            if (audioSource != null)
                audioSource.PlayOneShot(purchaseSound);
            else
                AudioSource.PlayClipAtPoint(purchaseSound, transform.position);
        }
    }

    void DisableShopPoint()
    {
        GetComponent<Collider>().enabled = false;
        if (purchasePrompt != null) purchasePrompt.SetActive(false);
        var renderer = GetComponent<Renderer>();
        if (renderer != null) renderer.enabled = false;
    }
}