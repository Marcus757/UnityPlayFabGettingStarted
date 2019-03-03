using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using PlayFab.DataModels;
using PlayFab.AuthenticationModels;

public class GameController : MonoBehaviour
{
    public Authentication authentication;
    public Dropdown functionDropdown;
    public GameObject parameters;
    public GameObject parameterPrefab;

    public Text scoreText;
    private int score;
    public GameObject loginPanel;
    public GameObject registrationPanel;
    public GameObject gamePanel;
    public GameObject errorPanel;
    public GameObject cube;
    private string username;
    private static string entityId;
    private static string entityType;
    private Dictionary<string, FunctionCaller> apiDictionary;

    // Use this for initialization
    void Start () {
        LoadApiDictionary();
        LoadDropDown();
        OnValueChangedDropDown();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void LoadApiDictionary()
    {
        apiDictionary = new Dictionary<string, FunctionCaller>();

        FunctionCaller loginGuest = new FunctionCaller() { };
        loginGuest.functionNoParams = authentication.Login;
        apiDictionary.Add("Login as guest", loginGuest);

        FunctionCaller loginUser = new FunctionCaller();
        loginUser.function2Params = (username, password) => authentication.Login(username, password);
        loginUser.parameters.Add("Username");
        loginUser.parameters.Add("Password");
        apiDictionary.Add("Login user with id", loginUser);

        FunctionCaller createAccount = new FunctionCaller();
        createAccount.function3Params = (email, username, password) => authentication.CreateAccount(email, username, password);
        createAccount.parameters.Add("Email");
        createAccount.parameters.Add("Username");
        createAccount.parameters.Add("Password");
        apiDictionary.Add("Create account with email", createAccount);
    }
    
    private void LoadDropDown()
    {
        functionDropdown.AddOptions(apiDictionary.Keys.ToList());
    }

    public void OnValueChangedDropDown()
    {
        DestroyChildren();
        FunctionCaller function = GetFunction();
        function.parameters.ForEach(param => 
        {
            GameObject instance = Instantiate(parameterPrefab);
            var placeholderText = instance.transform.Find("Placeholder").GetComponent<Text>();
            placeholderText.text = param;
            instance.transform.SetParent(parameters.transform);
        });
    }

    public void DestroyChildren()
    {
        foreach (Transform child in parameters.transform)
            Destroy(child.gameObject);
    }


    public void OnClickExecute()
    {
        FunctionCaller function = GetFunction();
        string param1, param2, param3;

        switch (function.parameters.Count)
        {
            case 1:
                param1 = parameters.transform.GetChild(0).Find("Text").GetComponent<Text>().text;
                function.function1Param.Invoke(param1);
                break;
            case 2:
                param1 = parameters.transform.GetChild(0).Find("Text").GetComponent<Text>().text;
                param2 = parameters.transform.GetChild(1).Find("Text").GetComponent<Text>().text;
                function.function2Params.Invoke(param1, param2);
                break;
            case 3:
                param1 = parameters.transform.GetChild(0).Find("Text").GetComponent<Text>().text;
                param2 = parameters.transform.GetChild(1).Find("Text").GetComponent<Text>().text;
                param3 = parameters.transform.GetChild(2).Find("Text").GetComponent<Text>().text;
                function.function3Params.Invoke(param1, param2, param3);
                break;
            default:
                function.functionNoParams.Invoke();
                break;
        }
    }

    private FunctionCaller GetFunction()
    {
        var functionName = functionDropdown.options.ElementAt(functionDropdown.value).text;
        return apiDictionary[functionName];
    }


    

    public void ShowGame()
    {
        GetEntityToken();
        var usernameText = gamePanel.GetComponentsInChildren<Text>().ToList().Find(x => x.name == "UsernameLabel");
        usernameText.text = "Username: " + username;
        cube.active = true;
    }

    public void ChangeColor()
    {
        Camera.main.backgroundColor = Random.ColorHSV();
    }

    public void SaveColor()
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
            Entity = new PlayFab.DataModels.EntityKey { Id = entityId, Type = entityType },
            Objects = dataList

        };
        PlayFabDataAPI.SetObjects(request, OnSetObjectsSuccess, OnSetObjectsFailure);
    }

    private void OnSetObjectsSuccess(SetObjectsResponse response)
    {
        Debug.Log(response.ProfileVersion);
    }

    private void OnSetObjectsFailure(PlayFabError error)
    {
        Debug.Log("SetObjects error");
        Debug.LogError(error.GenerateErrorReport());
    }

    private void GetEntityToken()
    {
        PlayFabAuthenticationAPI.GetEntityToken(new GetEntityTokenRequest(), OnGetEntityTokenSuccess, OnGetEntityTokenFailure);
    }

    private void OnGetEntityTokenSuccess(GetEntityTokenResponse response)
    {
        entityId = response.Entity.Id;
        entityType = response.Entity.Type;

        gamePanel.active = true;
        SetBackgroundColor();
    }

    private void OnGetEntityTokenFailure(PlayFabError error)
    {
        Debug.Log("GetEntityToken error");
        Debug.LogError(error.GenerateErrorReport());
    }

    private void SetBackgroundColor()
    {
        var request = new GetObjectsRequest
        {
            Entity = new PlayFab.DataModels.EntityKey { Id = entityId, Type = entityType }
        };

        PlayFabDataAPI.GetObjects(request, OnGetObjectsSuccess, OnGetObjectsFailure);
    }

    private void OnGetObjectsSuccess(GetObjectsResponse response)
    {
        ObjectResult result = null;
        response.Objects.TryGetValue("BackgroundColor", out result);

        if (result != null)
        {
            BackgroundColor backgroundColor = JsonUtility.FromJson<BackgroundColor>(result.DataObject.ToString());
            Camera.main.backgroundColor = new Color(backgroundColor.R, backgroundColor.G, backgroundColor.B, backgroundColor.A);
        }
    }

    private void OnGetObjectsFailure(PlayFabError error)
    {
        Debug.Log("OnGetObjects error");
        Debug.LogError(error.GenerateErrorReport());
    }

    public static string getEntityId()
    {
        return entityId;
    }

    public static string getEntityType()
    {
        return entityType;
    }





    public void OnScoreButtonClick()
    {
        UpdateScore();
    }

    private void UpdateScore()
    {
        score++;
        scoreText.text = "Score: " + score;
    }
}
