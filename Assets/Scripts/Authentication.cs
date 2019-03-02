using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Authentication : MonoBehaviour
{
    // Logs in user as a guest
    public void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = System.Guid.NewGuid().ToString(),
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnGuestLoginSuccess, OnSharedError);
    }

    // Logs in user with PlayFab id
    public void Login(string username, string password)
    {
        var request = new LoginWithPlayFabRequest { Username = username, Password = password };
        PlayFabClientAPI.LoginWithPlayFab(request, OnUserLoginSuccess, OnSharedError);
    }

    private void OnGuestLoginSuccess(LoginResult result)
    {
        Debug.Log("Guest login successful");
    }
    
    private void OnUserLoginSuccess(LoginResult result)
    {
        Debug.Log("User: " + result.PlayFabId + " login successful");
    }

    private void OnSharedError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
}
