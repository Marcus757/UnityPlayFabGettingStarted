using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class Statistics : MonoBehaviour {

    public void GetStatistics()
    {
        var request = new GetPlayerStatisticsRequest();
        PlayFabClientAPI.GetPlayerStatistics(request, OnGetPlayerStatisticsSuccess, OnGetPlayerStatisticsFailure);
    }

    private void OnGetPlayerStatisticsSuccess(GetPlayerStatisticsResult result)
    {
        Debug.Log("Successfully retrieved statistics");
        result.Statistics.ForEach(stat => Debug.Log("Statistic: " + stat.StatisticName + " - " + stat.Value));
    }

    private void OnGetPlayerStatisticsFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
}
