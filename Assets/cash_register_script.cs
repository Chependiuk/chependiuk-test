using UnityEngine;
using System.Collections;

public class CashRegister : MonoBehaviour
{
    [Header("Налаштування")]
    [SerializeField] private float moneyAmount = 150f;
    [SerializeField] private float cooldownTime = 10f;
    [SerializeField] private ParticleSystem moneyEffect;
    [SerializeField] private AudioClip moneySound;

    [Header("Індикатор")]
    [SerializeField] private MeshRenderer indicator;
    [SerializeField] private Material readyMaterial;
    [SerializeField] private Material cooldownMaterial;

    private bool isReady = true;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        UpdateIndicator();
    }

    public string GetInteractionText()
    {
        return isReady ? $"Отримати {moneyAmount}$ [E]" : "Каса оновлюється...";
    }

    public void Interact()
    {
        if (!isReady) return;

        GameManager.Instance.AddMoney(moneyAmount);
        PlayEffects();
        StartCoroutine(StartCooldown());
    }

    private IEnumerator StartCooldown()
    {
        isReady = false;
        UpdateIndicator();

        yield return new WaitForSeconds(cooldownTime);

        isReady = true;
        UpdateIndicator();
    }

    private void PlayEffects()
    {
        if (moneyEffect != null)
        {
            Instantiate(moneyEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        if (moneySound != null)
        {
            audioSource.PlayOneShot(moneySound);
        }
    }

    private void UpdateIndicator()
    {
        if (indicator != null)
        {
            indicator.material = isReady ? readyMaterial : cooldownMaterial;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(1f, 0.5f, 1f));
    }
}