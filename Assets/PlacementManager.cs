using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance { get; private set; }

    [Tooltip("Шар (Layer), на який можна ставити об'єкти, наприклад, підлога.")]
    [SerializeField] private LayerMask placementLayerMask;

    [Header("Налаштування обертання")]
    [Tooltip("Швидкість обертання об'єкта коліщатком миші.")]
    public float rotationSpeed = 150f;

    private GameObject objectToPlacePreview; // Об'єкт для попереднього перегляду
    private Camera mainCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (objectToPlacePreview != null)
        {
            MovePreviewToMouse();
            HandleRotation();

            if (Input.GetMouseButtonDown(0))
            {
                PlaceObject();
            }
            if (Input.GetMouseButtonDown(1))
            {
                CancelPlacing();
            }
        }
    }

    /// <summary>
    /// Починає режим розміщення. Викликається з ShopManager.
    /// </summary>
    public void StartPlacingItem(PlaceableItemData itemData)
    {
        if (objectToPlacePreview != null)
        {
            Destroy(objectToPlacePreview);
        }

        objectToPlacePreview = Instantiate(itemData.itemPrefab);

        // Вимикаємо керування гравцем
        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.SetMovementState(false);
        }

        if (CameraFollow.Instance != null)
        {
            CameraFollow.Instance.SetTemporaryTarget(objectToPlacePreview.transform);
        }
    }

    /// <summary>
    /// Фіксує об'єкт на сцені і повертає керування гравцю.
    /// </summary>
    private void PlaceObject()
    {
        if (CameraFollow.Instance != null)
        {
            CameraFollow.Instance.ClearTemporaryTarget();
        }

        // Вмикаємо керування гравцем
        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.SetMovementState(true);
        }

        objectToPlacePreview = null; // Виходимо з режиму розміщення
    }

    /// <summary>
    /// Скасовує розміщення і повертає керування гравцю.
    /// </summary>
    private void CancelPlacing()
    {
        if (CameraFollow.Instance != null)
        {
            CameraFollow.Instance.ClearTemporaryTarget();
        }

        // Вмикаємо керування гравцем
        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.SetMovementState(true);
        }

        Destroy(objectToPlacePreview);
        objectToPlacePreview = null;
    }

    /// <summary>
    /// Переміщує об'єкт-прев'ю за курсором миші.
    /// </summary>
    private void MovePreviewToMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, placementLayerMask))
        {
            objectToPlacePreview.transform.position = hit.point;
        }
    }

    /// <summary>
    /// Обертає об'єкт-прев'ю коліщатком миші.
    /// </summary>
    private void HandleRotation()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            objectToPlacePreview.transform.Rotate(Vector3.up, scrollInput * rotationSpeed * Time.deltaTime);
        }
    }
}