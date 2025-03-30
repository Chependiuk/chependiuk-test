using UnityEngine;

public class PlatformShopPoint : MonoBehaviour
{
    [Header("Налаштування")]
    public float cost = 500f;
    public GameObject platformPrefab; // Префаб платформи
    public Vector3 spawnPositionOffset = new Vector3(0, 0.5f, 0);

    [Header("Візуальні ефекти")]
    public GameObject purchasePrompt; // Текст "Натисніть E"
    public ParticleSystem purchaseEffect;
    public AudioClip purchaseSound;
    public float destroyDelay = 0.5f; // Затримка перед знищенням

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

        // Створення нової платформи
        Vector3 spawnPosition = transform.position + spawnPositionOffset;
        Instantiate(platformPrefab, spawnPosition, Quaternion.identity);

        // Відтворення ефектів
        PlayPurchaseEffects();

        // Вимкнення об'єкта
        DisableShopPoint();

        // Знищення об'єкта після затримки
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
        // Вимкнення всіх компонентів
        GetComponent<Collider>().enabled = false;
        if (purchasePrompt != null) purchasePrompt.SetActive(false);

        // Вимкнення візуальної частини
        var renderer = GetComponent<Renderer>();
        if (renderer != null) renderer.enabled = false;
    }
}