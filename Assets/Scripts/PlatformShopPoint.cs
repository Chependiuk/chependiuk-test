using UnityEngine;
using System.Collections.Generic;

public class PlatformShopPoint : MonoBehaviour
{
    [Header("������������")]
    public float cost = 100f;
    public GameObject[] platformPrefabs;
    public GameObject[] wallPrefabs;
    public float wallCheckDistance = 1.5f;

    [Header("������")]
    public ParticleSystem purchaseEffect;
    public AudioClip purchaseSound;

    public static List<GameObject> spawnedPlatforms = new List<GameObject>();
    public static List<GameObject> spawnedWalls = new List<GameObject>();
    public static List<Vector3> removedShopPoints = new List<Vector3>();
    public static GameObject[] staticPlatformPrefabs;
    public static GameObject[] staticWallPrefabs;

    private void Awake()
    {
        staticPlatformPrefabs = platformPrefabs;
        staticWallPrefabs = wallPrefabs;
    }

    public string GetInteractionText() => $"Buy territory ({cost}$) [E]";

    public void Interact()
    {
        if (GameManager.Instance.TrySpendMoney(cost))
        {
            BuildPlatform();
            PlayEffects();
            removedShopPoints.Add(transform.position);
            Destroy(gameObject);
        }
    }

    private void BuildPlatform()
    {
        if (platformPrefabs.Length == 0)
        {
            Debug.LogError("�� ������� ������� ��������!");
            return;
        }

        Vector3 spawnPos = new Vector3(transform.position.x, 0, transform.position.z);
        GameObject selectedPlatform = platformPrefabs[Random.Range(0, platformPrefabs.Length)];
        GameObject platform = Instantiate(selectedPlatform, spawnPos, Quaternion.identity);
        spawnedPlatforms.Add(platform);

        SpawnWalls(spawnPos, platform);
    }

    private void SpawnWalls(Vector3 platformPos, GameObject platform)
    {
        if (wallPrefabs.Length == 0) return;

        GameObject wallPrefab = wallPrefabs[Random.Range(0, wallPrefabs.Length)];
        Vector3 platformSize = platform.GetComponent<Renderer>().bounds.size;
        Vector3 wallSize = wallPrefab.GetComponent<Renderer>().bounds.size;

        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

        foreach (Vector3 dir in directions)
        {
            float rayLength = (dir.x != 0 ? platformSize.x : platformSize.z) / 2 + wallCheckDistance;
            if (!Physics.Raycast(platformPos, dir, rayLength))
            {
                Vector3 wallPos = platformPos;
                wallPos.y = wallSize.y / 2;

                if (dir.x != 0)
                    wallPos.x += dir.x * (platformSize.x / 2 + wallSize.x / 2);
                else
                    wallPos.z += dir.z * (platformSize.z / 2 + wallSize.z / 2);

                GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.LookRotation(dir));
                spawnedWalls.Add(wall);
            }
        }
    }

    private void PlayEffects()
    {
        if (purchaseEffect != null)
            Instantiate(purchaseEffect, transform.position, Quaternion.identity);

        if (purchaseSound != null)
            AudioSource.PlayClipAtPoint(purchaseSound, transform.position);
    }
}