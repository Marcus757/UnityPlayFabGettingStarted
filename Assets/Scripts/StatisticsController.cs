using PlayFab;
using PlayFab.ClientModels;
using PlayFab.ServerModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StatisticUpdate = PlayFab.ServerModels.StatisticUpdate;
using UpdatePlayerStatisticsRequest = PlayFab.ServerModels.UpdatePlayerStatisticsRequest;
using UpdatePlayerStatisticsResult = PlayFab.ServerModels.UpdatePlayerStatisticsResult;

public class StatisticsController : MonoBehaviour
{
    public void UpdatePlayerStatistics(string username, string password)
    {
        var statisticName1 = "Hourly High Score";
        var statisticName2 = "Overall High Score";

        var request = new LoginWithPlayFabRequest { Username = username, Password = password };
        PlayFabClientAPI.LoginWithPlayFab(request,
            delegate (LoginResult loginResult)
            {
                var updatePlayerStatisticsRequest = new UpdatePlayerStatisticsRequest()
                {
                    PlayFabId = loginResult.PlayFabId,
                    Statistics = new List<StatisticUpdate>()
                    {
                        new StatisticUpdate(){ StatisticName = statisticName1, Value = GameController.score },
                        new StatisticUpdate(){ StatisticName = statisticName2, Value = GameController.score }
                    }
                };

                PlayFabServerAPI.UpdatePlayerStatistics(updatePlayerStatisticsRequest,
                    delegate (UpdatePlayerStatisticsResult updatePlayerStatisticsResult)
                    {
                        Debug.Log("Statistic: " + statisticName1 + " and Statistic: " + statisticName2 + " were updated");
                    },
                    SharedError.OnSharedError);
            
            },
            SharedError.OnSharedError);
    }
}
