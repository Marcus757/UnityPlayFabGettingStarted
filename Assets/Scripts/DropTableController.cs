using PlayFab;
using PlayFab.ClientModels;
using PlayFab.ServerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropTableController : MonoBehaviour
{
    public void ReadDropTableData(string dropTableId)
    {
        var getRandomResultTables = new GetRandomResultTablesRequest() { TableIDs = new List<string> { dropTableId } };
        PlayFabServerAPI.GetRandomResultTables(getRandomResultTables,
            delegate (GetRandomResultTablesResult getRandomResultTablesResult)
            {
                foreach (var result in getRandomResultTablesResult.Tables.ToList())
                {
                    Debug.Log("Drop table id: " + result.Key);

                    foreach (var node in result.Value.Nodes)
                    {
                        Debug.Log("Id: " + node.ResultItem + ", Weight: " + node.Weight);
                    }
                } 
            },
            SharedError.OnSharedError);
    }

    public void GrantRandomItemToUser(string adminUsername, string password, string usernameReceivingItem, string dropTableId)
    {
        GameController.catalog.Clear();
        var loginWithPlayFabRequest = new LoginWithPlayFabRequest { Username = adminUsername, Password = password };

        PlayFabClientAPI.LoginWithPlayFab(loginWithPlayFabRequest,
            delegate (LoginResult loginResult)
            {
                var evaluateRandomResultTableRequest = new EvaluateRandomResultTableRequest() { TableId = dropTableId };

                PlayFabServerAPI.EvaluateRandomResultTable(evaluateRandomResultTableRequest,
                    delegate (EvaluateRandomResultTableResult evaluateRandomResultTableResult)
                    {
                        var getAccountInfoRequest = new GetAccountInfoRequest() { Username = usernameReceivingItem };

                        PlayFabClientAPI.GetAccountInfo(getAccountInfoRequest,
                            delegate (GetAccountInfoResult getAccountInfoResult)
                            {
                                var grantItemsToUserRequest = new GrantItemsToUserRequest()
                                {
                                    PlayFabId = getAccountInfoResult.AccountInfo.PlayFabId,
                                    ItemIds = new List<string> { evaluateRandomResultTableResult.ResultItemId }
                                };

                                PlayFabServerAPI.GrantItemsToUser(grantItemsToUserRequest,
                                    delegate (GrantItemsToUserResult grantItemsToUser)
                                    {

                                        PlayFabClientAPI.GetCatalogItems(new PlayFab.ClientModels.GetCatalogItemsRequest(),
                                            delegate (PlayFab.ClientModels.GetCatalogItemsResult getCatalogItemsResult)
                                            {
                                                getCatalogItemsResult.Catalog.ForEach(catalogItem =>
                                                {
                                                    GameController.catalog.Add(catalogItem.ItemId, catalogItem);
                                                });

                                                Debug.Log("Items granted:");
                                                grantItemsToUser.ItemGrantResults.ForEach(item =>
                                                {
                                                    Debug.Log(item.ItemId);
                                                    AddPowerUp(item);
                                                });
                                            },
                                            SharedError.OnSharedError);
                                    },
                                    SharedError.OnSharedError);
                            },
                            SharedError.OnSharedError);
                    },
                    SharedError.OnSharedError);
            },
            SharedError.OnSharedError);
    }

    private void AddPowerUp(GrantedItemInstance item)
    {
        PlayFab.ClientModels.CatalogItem catalogItem;
        GameController.catalog.TryGetValue(item.ItemId, out catalogItem);

        if (catalogItem != null)
        {
            PowerUp powerUp = new PowerUp();
            powerUp.itemId = catalogItem.ItemId;
            powerUp.displayName = catalogItem.DisplayName;
            powerUp.expirationDateTime = item.Expiration;
            powerUp.multiplierAmount = JsonUtility.FromJson<MultiplerAmount>(catalogItem.CustomData).multiplierAmount;
            GameController.powerUps.Add(powerUp);
        }
    }
}
