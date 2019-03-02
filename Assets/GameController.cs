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
        //scoreText.text = "Score: " + score;
                
        LoadApiDictionary();
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
        apiDictionary["Login as guest"].functionNoParams.Invoke();

        FunctionCaller loginUser = new FunctionCaller();
        loginUser.function2Params = (username, password) => authentication.Login(username, password);
        apiDictionary.Add("Login user with id", loginUser);
        apiDictionary["Login user with id"].function2Params.Invoke("Marcus757", "uncc2007");
    }
    
    public void CreateAccount()
    {
        var emailField = registrationPanel.transform.Find("EmailInputField").GetComponent<InputField>();
        var usernameField = registrationPanel.transform.Find("UsernameInputField").GetComponent<InputField>();
        var passwordField = registrationPanel.transform.Find("PasswordInputField").GetComponent<InputField>();

        var request = new AddUsernamePasswordRequest
        {
            Email = emailField.text,
            Username = usernameField.text,
            Password = passwordField.text
        };
        PlayFabClientAPI.AddUsernamePassword(request, OnAddUsernamePasswordSuccess, OnAddUsernamePasswordFailure);
    }

    private void OnAddUsernamePasswordSuccess(AddUsernamePasswordResult result)
    {
        registrationPanel.active = false;
        username = result.Username;
        ShowGame();
    }

    private void OnAddUsernamePasswordFailure(PlayFabError error)
    {
        Debug.Log("Create account failure");
        Debug.LogError(error.GenerateErrorReport());
        registrationPanel.active = false;
        errorPanel.active = true;
        ShowError(error);
    }

    private void ShowError(PlayFabError error)
    {
        var errorMessage = errorPanel.transform.Find("ErrorMessage").GetComponent<Text>();
        errorMessage.text = error.ErrorMessage;
    }

    public void ContinueAsGuest()
    {
        registrationPanel.active = false;
        ShowGame();
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
