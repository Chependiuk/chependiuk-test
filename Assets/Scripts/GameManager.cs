using UnityEngine;
using TMPro;
using System.IO;

// Додаємо вимогу, щоб на об'єкті був компонент AudioSource
[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public TextMeshProUGUI moneyText;

    public float PlayerMoney { get; private set; } = 0f;

    [Header("Аудіо")]
    [SerializeField] private AudioClip moneyAddSound;

    private AudioSource audioSource;

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
            // Отримуємо компонент AudioSource з цього ж об'єкта
            audioSource = GetComponent<AudioSource>();

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
        if (amount <= 0) return; // Не додаємо нуль або від'ємні значення

        PlayerMoney += amount;
        UpdateMoneyUI();

        // Відтворюємо звук, якщо він є
        if (moneyAddSound != null)
        {
            audioSource.PlayOneShot(moneyAddSound);
        }

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

    public void SaveGame()
    {
        SaveData data = new SaveData
        {
            playerMoney = PlayerMoney
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            PlayerMoney = data.playerMoney;
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}