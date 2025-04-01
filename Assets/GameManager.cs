using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public TextMeshProUGUI moneyText;
    private float money = 1000f;

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

    public bool TrySpendMoney(float amount)
    {
        if (money < amount) return false;

        money -= amount;
        UpdateMoneyUI();
        return true;
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"Ãðîø³: {money}$";
    }
}