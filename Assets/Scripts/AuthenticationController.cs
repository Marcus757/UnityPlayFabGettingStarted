﻿using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class AuthenticationController : MonoBehaviour
{

    // Logs in user as a guest
    public void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = System.Guid.NewGuid().ToString(),
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, 
            delegate(LoginResult result)
            {
                Debug.Log("Guest login successful");
            }, 
            SharedError.OnSharedError);
    }

    // Logs in user with PlayFab id
    public void Login(string username, string password)
    {
        var request = new LoginWithPlayFabRequest { Username = username, Password = password };
        PlayFabClientAPI.LoginWithPlayFab(request, 
            delegate (LoginResult result)
            {
                Debug.Log("User: " + result.PlayFabId + " login successful");
            },
            SharedError.OnSharedError);
    }
    

    public void CreateAccount(string email, string username, string password)
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = System.Guid.NewGuid().ToString(),
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request,
            delegate (LoginResult result)
            {
                var secondRequest = new AddUsernamePasswordRequest
                {
                    Email = email,
                    Username = username,
                    Password = password
                };
                PlayFabClientAPI.AddUsernamePassword(secondRequest, OnAddUsernamePasswordSuccess, SharedError.OnSharedError);
            }, 
            SharedError.OnSharedError);
    }

    private void OnAddUsernamePasswordSuccess(AddUsernamePasswordResult result)
    {
        var prevRequest = (AddUsernamePasswordRequest)result.Request;
        Debug.Log("Username: " + prevRequest.Username + " with Email:" + prevRequest.Email + " has been created successfully");
    }
}
