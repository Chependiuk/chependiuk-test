using UnityEngine;
using TMPro;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public TextMeshProUGUI moneyText;

    public float PlayerMoney { get; private set; } = 0f;
    [Header("Аудіо")]
    [SerializeField] private AudioClip moneyAddSound;

    // Клас для серіалізації даних
    [System.Serializable]
    private class SaveData
    {
        public float playerMoney;
    }

    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            savePath = Path.Combine(Application.persistentDataPath, "saveData.json");
            LoadGame(); // Завантажуємо дані при старті
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
        SaveGame(); // Зберігаємо при зміні кількості грошей
    }

    public bool TrySpendMoney(float amount)
    {
        if (PlayerMoney >= amount)
        {
            PlayerMoney -= amount;
            UpdateMoneyUI();
            SaveGame(); // Зберігаємо при витраті грошей
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

    // Метод для збереження гри
    public void SaveGame()
    {
        SaveData data = new SaveData
        {
            playerMoney = PlayerMoney
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
    }

    // Метод для завантаження гри
    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            PlayerMoney = data.playerMoney;
        }
    }

    // Для тестування можна додати збереження при закритті гри
    private void OnApplicationQuit()
    {
        SaveGame();
    }
}