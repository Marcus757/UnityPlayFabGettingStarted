﻿using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    public void PurchaseItem(string username, string password, string itemId, string price)
    {
        var loginWithPlayFabRequest = new LoginWithPlayFabRequest { Username = username, Password = password };

        PlayFabClientAPI.LoginWithPlayFab(loginWithPlayFabRequest,
            delegate (LoginResult loginResult)
            {
                var purchaseItemRequest = new PurchaseItemRequest()
                {
                    ItemId = itemId,
                    Price = Convert.ToInt32(price),
                    VirtualCurrency = "MC"
                };

                PlayFabClientAPI.PurchaseItem(purchaseItemRequest,
                    delegate (PurchaseItemResult purchaseItemResult)
                    {
                        Debug.Log("Item Purchased: ");
                        purchaseItemResult.Items.ForEach(item =>
                        {
                            Debug.Log("ItemId: " + item.ItemId + ", Price: " + item.UnitPrice + ", VirtualCurrency: " + item.UnitCurrency);
                        });
                    },
                    SharedError.OnSharedError);
            },
            SharedError.OnSharedError);
    }

    public void GetInventory(string username, string password)
    {
        var loginWithPlayFabRequest = new LoginWithPlayFabRequest { Username = username, Password = password };

        PlayFabClientAPI.LoginWithPlayFab(loginWithPlayFabRequest,
            delegate (LoginResult loginResult)
            {
                PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
                delegate (GetUserInventoryResult getUserInventoryResult)
                {
                    Debug.Log("Player Inventory");
                    getUserInventoryResult.Inventory.ForEach(item =>
                    {
                        Debug.Log("ItemId: " + item.ItemId + ", Price: " + item.UnitPrice + ", VirtualCurrency: " + item.UnitCurrency);
                    });
                },
                SharedError.OnSharedError);
            },
            SharedError.OnSharedError);
    }

    public void ConsumeItem(string username, string password, string itemId)
    {
        var loginWithPlayFabRequest = new LoginWithPlayFabRequest { Username = username, Password = password };

        PlayFabClientAPI.LoginWithPlayFab(loginWithPlayFabRequest,
            delegate (LoginResult loginResult)
            {
                PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
                delegate (GetUserInventoryResult getUserInventoryResult)
                {
                    var inventoryItem = getUserInventoryResult.Inventory.Find(item => item.ItemId == itemId);
                    if (inventoryItem != null)
                    {
                        var consumeItemRequest = new ConsumeItemRequest() { ConsumeCount = 1, ItemInstanceId = inventoryItem.ItemInstanceId };

                        PlayFabClientAPI.ConsumeItem(consumeItemRequest,
                            delegate (ConsumeItemResult consumeItemResult)
                            {
                                Debug.Log("Item Consumed: " + consumeItemResult.ItemInstanceId);
                            },
                            SharedError.OnSharedError);
                    }
                    else
                    {
                        Debug.Log("Unable to locate itemId: " + itemId);
                    }
                },
                SharedError.OnSharedError);
            },
            SharedError.OnSharedError);
    }
}