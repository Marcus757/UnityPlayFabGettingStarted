using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class Automation : MonoBehaviour {

	public void ExecuteCloudScript()
    {
        var request = new ExecuteCloudScriptRequest {  FunctionName = "bushelOnYourFirstDay", GeneratePlayStreamEvent = true };
        PlayFabClientAPI.ExecuteCloudScript(request, OnExecuteCloudScriptSuccess, OnExecuteCloudScriptFailure);
    }

    private void OnExecuteCloudScriptSuccess(ExecuteCloudScriptResult result)
    {
        Debug.Log("Execute Cloud Script Success");
    }

    private void OnExecuteCloudScriptFailure(PlayFabError error)
    {
        Debug.LogError("Execute Cloud Script Failure");
        Debug.LogError(error.GenerateErrorReport());
    }
}
