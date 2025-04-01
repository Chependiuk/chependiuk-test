using UnityEngine;

public class PlatformShopPoint : MonoBehaviour
{
    [Header("Settings")]
    public float cost = 100f;
    public GameObject platformPrefab;
    public GameObject[] wallPrefabs;
    public float wallCheckDistance = 1.5f;

    [Header("Effects")]
    public ParticleSystem purchaseEffect;
    public AudioClip purchaseSound;
    public float destroyDelay = 0.5f;

    private bool isPurchased = false;

    public string GetInteractionText() => $" упити ст≥ну ({cost}$) [E]";

    public void Interact()
    {
        if (isPurchased || !GameManager.Instance.TrySpendMoney(cost)) return;

        isPurchased = true;
        BuildPlatform();
        PlayEffects();
        Destroy(gameObject, destroyDelay);
    }

    private void BuildPlatform()
    {
        Vector3 spawnPosition = new Vector3(transform.position.x, 0, transform.position.z);
        GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
        SpawnWalls(spawnPosition, platform);
    }

    private void SpawnWalls(Vector3 platformPosition, GameObject platform)
    {
        if (wallPrefabs == null || wallPrefabs.Length == 0) return;

        GameObject wallToSpawn = wallPrefabs[Random.Range(0, wallPrefabs.Length)];
        Vector3 platformSize = platform.GetComponent<Renderer>().bounds.size;
        Vector3 wallSize = wallToSpawn.GetComponent<Renderer>().bounds.size;

        Vector3[] directions = { Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        foreach (Vector3 dir in directions)
        {
            CheckAndSpawnWall(platformPosition, dir, platformSize, wallSize, wallToSpawn);
        }
    }

    private void CheckAndSpawnWall(Vector3 platformPosition, Vector3 direction, Vector3 platformSize, Vector3 wallSize, GameObject wallPrefab)
    {
        float rayLength = direction.x != 0 ?
            platformSize.x / 2 + wallCheckDistance :
            platformSize.z / 2 + wallCheckDistance;

        if (!Physics.Raycast(platformPosition, direction, rayLength))
        {
            Vector3 wallPosition = platformPosition;
            wallPosition.y = wallSize.y / 2;

            if (direction.x != 0)
            {
                wallPosition.x += direction.x * (platformSize.x / 2 + wallSize.x / 2);
            }
            else
            {
                wallPosition.z += direction.z * (platformSize.z / 2 + wallSize.z / 2);
            }

            Instantiate(wallPrefab, wallPosition, Quaternion.LookRotation(direction));
        }
    }

    private void PlayEffects()
    {
        if (purchaseEffect != null)
            Instantiate(purchaseEffect, transform.position, Quaternion.identity);

        if (purchaseSound != null)
            AudioSource.PlayClipAtPoint(purchaseSound, transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}