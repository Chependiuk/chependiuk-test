using UnityEngine;
using System.IO;
using static InteractableObject;

[System.Serializable]
public class GameData
{
    public float money;
    // Тут можна додати інші дані для збереження:
    // public List<string> purchasedItems;
    // public int levelProgress;
}

public class main_player_script : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSpeed = 10f;
    public DynamicJoystick dynamicJoystick;
    public Rigidbody rb;

    [Header("UI References")]
    public TMPro.TextMeshProUGUI moneyText;

    [Header("Interaction Settings")]
    public float closeInteractionDistance = 2f;
    public float farInteractionDistance = 4f;
    public LayerMask interactableLayer;

    private InteractableObject currentInteractable;
    private GameData gameData;
    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "game_save.json");
        LoadGameData();
    }

    void Start()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        UpdateMoneyUI();
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SaveGameData();
    }

    #region Save/Load System
    private void SaveGameData()
    {
        string jsonData = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(savePath, jsonData);
        Debug.Log("Game data saved to: " + savePath);
    }

    private void LoadGameData()
    {
        if (File.Exists(savePath))
        {
            string jsonData = File.ReadAllText(savePath);
            gameData = JsonUtility.FromJson<GameData>(jsonData);
            Debug.Log("Game data loaded successfully");
        }
        else
        {
            gameData = new GameData { money = 0f };
            Debug.Log("New game data created");
        }
    }

    public void ResetGameData()
    {
        gameData = new GameData { money = 0f };
        SaveGameData();
        UpdateMoneyUI();
    }
    #endregion

    #region Money System
    public float PlayerMoney
    {
        get => gameData.money;
        set
        {
            gameData.money = value;
            UpdateMoneyUI();
            SaveGameData(); // Автозбереження при зміні
        }
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"Гроші: {PlayerMoney}$";
    }
    #endregion

    #region Player Movement
    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 direction = new Vector3(dynamicJoystick.Horizontal, 0, dynamicJoystick.Vertical);
        if (direction.magnitude > 0.1f)
        {
            rb.AddForce(direction.normalized * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
    #endregion

    #region Interaction System
    private void Update()
    {
        CheckInteractables();

        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null &&
            GetDistanceTo(currentInteractable.transform) <= closeInteractionDistance)
        {
            InteractWithObject();
        }
    }

    private void CheckInteractables()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, farInteractionDistance, interactableLayer);
        InteractableObject nearestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider col in hitColliders)
        {
            InteractableObject interactable = col.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                float distance = GetDistanceTo(col.transform);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestObject = interactable;
                }
            }
        }

        if (currentInteractable != nearestObject)
        {
            if (currentInteractable != null)
                currentInteractable.SetInteractState(InteractState.None);

            currentInteractable = nearestObject;
        }

        if (currentInteractable != null)
        {
            float distance = GetDistanceTo(currentInteractable.transform);
            if (distance <= closeInteractionDistance)
            {
                currentInteractable.SetInteractState(InteractState.Active);
            }
            else if (distance <= farInteractionDistance)
            {
                currentInteractable.SetInteractState(InteractState.Near);
            }
            else
            {
                currentInteractable.SetInteractState(InteractState.None);
                currentInteractable = null;
            }
        }
    }

    private float GetDistanceTo(Transform target)
    {
        return Vector3.Distance(transform.position, target.position);
    }

    private void InteractWithObject()
    {
        if (currentInteractable.CompareTag("CashRegister"))
        {
            PlayerMoney += 100f; // Використовуємо властивість для авто-збереження
        }
        // Додаткові типи взаємодій можна додати тут
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, closeInteractionDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, farInteractionDistance);
    }
    public bool TrySpendMoney(float amount)
    {
        if (PlayerMoney >= amount)
        {
            PlayerMoney -= amount;
            return true;
        }
        return false;
    }
}