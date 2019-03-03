using PlayFab;
using PlayFab.AuthenticationModels;
using PlayFab.DataModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundColorController : MonoBehaviour
{
    public void ChangeBackgroundColor()
    {
        Camera.main.backgroundColor = Random.ColorHSV();
    }

    public void SaveBackgroundColor()
    {
        PlayFabAuthenticationAPI.GetEntityToken(new GetEntityTokenRequest(), OnGetEntityTokenForSaveBackgroundColor, OnSharedError);
    }

    public void GetBackgroundColor()
    {
        PlayFabAuthenticationAPI.GetEntityToken(new GetEntityTokenRequest(), OnGetEntityTokenForGetBackgroundColor, OnSharedError);
    }

    private void OnGetEntityTokenForSaveBackgroundColor(GetEntityTokenResponse getEntityTokenResponse)
    {
        var data = new Dictionary<string, object>()
        {
            { "R", Camera.main.backgroundColor.r },
            { "G", Camera.main.backgroundColor.g },
            { "B", Camera.main.backgroundColor.b },
            { "A", Camera.main.backgroundColor.a },
        };

        var dataList = new List<SetObject>()
        {
            new SetObject()
            {
                ObjectName = "BackgroundColor",
                DataObject = data
            }
        };

        var request = new SetObjectsRequest()
        {
            Entity = new PlayFab.DataModels.EntityKey { Id = getEntityTokenResponse.Entity.Id, Type = getEntityTokenResponse.Entity.Type },
            Objects = dataList

        };

        PlayFabDataAPI.SetObjects(request,
            delegate (SetObjectsResponse setObjectsResponse)
            {
                Debug.Log("Background color has been saved");
            },
            OnSharedError);
    }

    private void OnGetEntityTokenForGetBackgroundColor(GetEntityTokenResponse response)
    {
        var request = new GetObjectsRequest
        {
            Entity = new PlayFab.DataModels.EntityKey { Id = response.Entity.Id, Type = response.Entity.Type }
        };

        PlayFabDataAPI.GetObjects(request, OnGetObjectsSuccess, OnSharedError);
    }

    private void OnGetObjectsSuccess(GetObjectsResponse response)
    {
        Debug.Log("Background color was retrieved successfully");

        ObjectResult result = null;
        response.Objects.TryGetValue("BackgroundColor", out result);

        if (result != null)
        {
            BackgroundColor backgroundColor = JsonUtility.FromJson<BackgroundColor>(result.DataObject.ToString());
            Camera.main.backgroundColor = new Color(backgroundColor.R, backgroundColor.G, backgroundColor.B, backgroundColor.A);
        }
    }

    private void OnSharedError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
}
