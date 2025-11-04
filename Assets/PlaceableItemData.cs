using UnityEngine;

[CreateAssetMenu(fileName = "New Placeable Item", menuName = "MyGame/Placeable Item")]
public class PlaceableItemData : ScriptableObject
{
    [Header("Дані для магазину")]
    public string itemName;
    public Sprite itemIcon;
    public int cost;

    [Header("Дані для розміщення")]
    public GameObject itemPrefab; // 3D-модель (префаб), яка буде ставитися на сцену
}