using PlayFab;
using PlayFab.AdminModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIAccessPolicy : MonoBehaviour
{
    public void FetchApiPolicy()
    {
        var getPolicyRequest = new PlayFab.AdminModels.GetPolicyRequest() { PolicyName = "ApiPolicy" };
        PlayFabAdminAPI.GetPolicy(getPolicyRequest,
            delegate (GetPolicyResponse getPolicyResponse)
            {
                Debug.Log(getPolicyResponse.PolicyName);

                foreach (var statement in getPolicyResponse.Statements)
                {
                    Debug.Log("Action: " + statement.Action);
                    Debug.Log("Comment: " + statement.Comment);

                    if (statement.ApiConditions != null)
                    {
                        Debug.Log("ApiConditions.HasSignatureOrEncryption: " + statement.ApiConditions.HasSignatureOrEncryption);
                    }
                    Debug.Log("Effect: " + statement.Effect);
                    Debug.Log("Principal: " + statement.Principal);
                    Debug.Log("Resource: " + statement.Resource);
                }
            },
            OnSharedError);
    }

    private void OnSharedError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
}
