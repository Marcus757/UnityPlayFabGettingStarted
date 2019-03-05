using PlayFab;
using UnityEngine;

public static class SharedError
{
    public static void OnSharedError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
}
