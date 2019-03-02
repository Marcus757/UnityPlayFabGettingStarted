using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class Player : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        //Note: Setting title Id here can be skipped if you have set the value in Editor Extensions already.
        //if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        //    PlayFabSettings.TitleId = "144"; // Please change this value to your own titleId from PlayFab Game Manager

        //var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuid", CreateAccount = true };
        //PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
	}
	
	private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login Successful");

        //GetComponent<Statistics>().GetStatistics();
        //GetComponent<Catalog>().GetItems();
        //GetComponent<Catalog>().GetStoreItems();
        //GetComponent<Automation>().ExecuteCloudScript();
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Login Failure");
        Debug.LogError(error.GenerateErrorReport());
    }
}
