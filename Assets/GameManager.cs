using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public TextMeshProUGUI moneyText;

    public float PlayerMoney { get; private set; } = 0f;
    [Header("Àóä³î")]
    [SerializeField] private AudioClip moneyAddSound;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            UpdateMoneyUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMoney(float amount)
    {
        PlayerMoney += amount;
        UpdateMoneyUI();
    }

    public bool TrySpendMoney(float amount)
    {
        if (PlayerMoney >= amount)
        {
            PlayerMoney -= amount;
            UpdateMoneyUI();
            return true;
        }
        return false;
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"Money: {PlayerMoney:F0}$";
        }
    }
}