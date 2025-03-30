using UnityEngine;

public class PlatformShopPoint : MonoBehaviour
{
    [Header("������������")]
    public float cost = 500f;
    public GameObject platformPrefab; // ������ ���������
    public Vector3 spawnPositionOffset = new Vector3(0, 0.5f, 0);

    [Header("³������ ������")]
    public GameObject purchasePrompt; // ����� "�������� E"
    public ParticleSystem purchaseEffect;
    public AudioClip purchaseSound;
    public float destroyDelay = 0.5f; // �������� ����� ���������

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

        // ��������� ���� ���������
        Vector3 spawnPosition = transform.position + spawnPositionOffset;
        Instantiate(platformPrefab, spawnPosition, Quaternion.identity);

        // ³��������� ������
        PlayPurchaseEffects();

        // ��������� ��'����
        DisableShopPoint();

        // �������� ��'���� ���� ��������
        Destroy(gameObject, destroyDelay);
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
        // ��������� ��� ����������
        GetComponent<Collider>().enabled = false;
        if (purchasePrompt != null) purchasePrompt.SetActive(false);

        // ��������� �������� �������
        var renderer = GetComponent<Renderer>();
        if (renderer != null) renderer.enabled = false;
    }
}