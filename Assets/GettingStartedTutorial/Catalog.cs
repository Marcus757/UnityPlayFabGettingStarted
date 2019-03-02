using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;

public class Catalog : MonoBehaviour
{
    public void GetItems()
    {
        var request = new GetCatalogItemsRequest { CatalogVersion = "main" };
        PlayFabClientAPI.GetCatalogItems(request, OnGetCatalogItemsSuccess, OnGetCatalogItemsFailure);
    }

    public void GetStoreItems()
    {
        var request = new GetStoreItemsRequest { CatalogVersion = "main", StoreId = "fruits" };
        PlayFabClientAPI.GetStoreItems(request, OnGetStoreItemsSuccess, OnGetStoreItemsFailure);
    }

    private void OnGetCatalogItemsSuccess(GetCatalogItemsResult result)
    {
        Debug.Log("Catalog items retrieved successfully");

        var item = result.Catalog.Find(x => x.ItemId == "apple");

        if (item != null)
        {
            var request = new PurchaseItemRequest
            {
                ItemId = item.ItemId,
                CatalogVersion = item.CatalogVersion,
                VirtualCurrency = "GO",
                Price = Convert.ToInt32(item.VirtualCurrencyPrices["GO"])
            };

            PlayFabClientAPI.PurchaseItem(request, OnPurchaseItemSuccess, OnPurchaseItemFailure);
        }
    }

    private void OnGetCatalogItemsFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnPurchaseItemSuccess(PurchaseItemResult result)
    {
        Debug.Log("Items Purchased");
        result.Items.ForEach(Print);

        var request = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(request, OnGetUserInventorySuccess, OnGetUserInventoryFailure);
    }

    private void OnPurchaseItemFailure(PlayFabError error)
    {
        Debug.LogError("Purchase Item Failure");
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnGetUserInventorySuccess(GetUserInventoryResult result)
    {
        Debug.Log("Get Inventory");
        Debug.Log("Inventory Count: " + result.Inventory.Count);
        result.Inventory.ForEach(Print);
    }

    private void OnGetUserInventoryFailure(PlayFabError error)
    {
        Debug.LogError("Get Inventory Failure");
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnGetStoreItemsSuccess(GetStoreItemsResult result)
    {
        Debug.Log("Get Store Items");
        result.Store.ForEach(Print);

        StoreItem item = result.Store.Find(storeItem => storeItem.ItemId == "pear");
        if (item != null)
        {
            var request = new PurchaseItemRequest
            {
                CatalogVersion = "main",
                StoreId = "fruits",
                ItemId = item.ItemId,
                VirtualCurrency = "GO",
                Price = Convert.ToInt32(item.VirtualCurrencyPrices["GO"])
            };
            PlayFabClientAPI.PurchaseItem(request, OnPurchaseItemSuccess, OnPurchaseItemFailure);
        }

    }

    private void OnGetStoreItemsFailure(PlayFabError error)
    {
        Debug.LogError("Get Store Items Failure");
        Debug.LogError(error.GenerateErrorReport());
    }

    private void Print(ItemInstance item)
    {
        Debug.Log("DisplayName: " + item.DisplayName + 
            ", ItemId: " + item.ItemId + 
            ", CatalogVersion: " + item.CatalogVersion + 
            ", UnitPrice: " + item.UnitPrice);
    }

    private void Print(StoreItem item)
    {
        Debug.Log("ItemId: " + item.ItemId + 
            ", VirtualCurrency: " + item.VirtualCurrencyPrices["GO"]);
    }
}
