using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public PlaceableItemData itemToSell;

    public void OnBuyButtonClick()
    {
        if (itemToSell == null)
        {
            Debug.LogError("До кнопки не прикріплено товар (ItemToSell)!", this.gameObject);
            return;
        }

        if (GameManager.Instance.TrySpendMoney(itemToSell.cost))
        {
            Debug.Log($"Ви успішно купили {itemToSell.itemName}!");
            PlacementManager.Instance.StartPlacingItem(itemToSell);

            // Закриваємо вікно магазину після покупки
            transform.root.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Недостатньо грошей!");
        }
    }
}