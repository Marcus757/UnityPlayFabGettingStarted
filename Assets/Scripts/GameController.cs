using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour
{
    public Dropdown functionDropdown;
    public GameObject parameters;
    public GameObject parameterPrefab;
    public AuthenticationController authentication;
    public BackgroundColorController backgroundColorChanger;
    public GroupController groupController;
    public APIAccessPolicyController apiAccessPolicyController;
    public DropTableController dropTableController;
    public PlayerInventoryController playerInventoryController;
    public GameObject cube;
    public Text scoreText;

    private Dictionary<string, FunctionCaller> apiDictionary;
    private int score;
    private static int scoreMultiplier = 1;

    // Use this for initialization
    void Start () {
        LoadApiDictionary();
        LoadDropDown();
        OnValueChangedDropDown();
        cube.SetActive(true);
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

        FunctionCaller changeBackgroundColor = new FunctionCaller();
        changeBackgroundColor.functionNoParams = ()=> backgroundColorChanger.ChangeBackgroundColor();
        apiDictionary.Add("Change background color", changeBackgroundColor);

        FunctionCaller saveBackgroundColor = new FunctionCaller();
        saveBackgroundColor.functionNoParams = () => backgroundColorChanger.SaveBackgroundColor();
        apiDictionary.Add("Save background color", saveBackgroundColor);

        FunctionCaller getBackgroundColor = new FunctionCaller();
        getBackgroundColor.functionNoParams = () => backgroundColorChanger.GetBackgroundColor();
        apiDictionary.Add("Get background color", getBackgroundColor);

        FunctionCaller createGroup = new FunctionCaller();
        createGroup.function3Params = (adminUsername, password, groupName) => groupController.CreateGroup(adminUsername, password, groupName);
        createGroup.parameters.Add("Admin Username");
        createGroup.parameters.Add("Password");
        createGroup.parameters.Add("Group Name");
        apiDictionary.Add("Create group", createGroup);

        FunctionCaller invitePlayerToGroup = new FunctionCaller();
        invitePlayerToGroup.function4Params = 
            (adminUsername, password, groupName, usernameToInvite) => 
            groupController.InvitePlayerToGroup(adminUsername, password, groupName, usernameToInvite);
        invitePlayerToGroup.parameters.Add("Admin Username");
        invitePlayerToGroup.parameters.Add("Password");
        invitePlayerToGroup.parameters.Add("Group Name");
        invitePlayerToGroup.parameters.Add("Username To Invite");
        apiDictionary.Add("Invite player to group", invitePlayerToGroup);

        FunctionCaller acceptInvitationToGroup = new FunctionCaller();
        acceptInvitationToGroup.function3Params = 
            (usernameToAccept, password, groupName) => 
            groupController.AcceptInvitationToGroup(usernameToAccept, password, groupName);
        acceptInvitationToGroup.parameters.Add("Username To Accept");
        acceptInvitationToGroup.parameters.Add("Password");
        acceptInvitationToGroup.parameters.Add("Group Name");
        apiDictionary.Add("Accept invitation to group", acceptInvitationToGroup);

        FunctionCaller fetchApiPolicy = new FunctionCaller();
        fetchApiPolicy.functionNoParams = () => apiAccessPolicyController.FetchApiPolicy();
        apiDictionary.Add("Fetch API access policy", fetchApiPolicy);

        FunctionCaller readDropTableData = new FunctionCaller();
        readDropTableData.function1Param = (tableId) => dropTableController.ReadDropTableData(tableId);
        readDropTableData.parameters.Add("Drop table id");
        apiDictionary.Add("Read drop table data", readDropTableData);

        FunctionCaller grantRandomItemToUser = new FunctionCaller();
        grantRandomItemToUser.function4Params = 
            (adminUsername, password, usernameReceivingItem, dropTableId) => 
            dropTableController.GrantRandomItemToUser(adminUsername, password, usernameReceivingItem, dropTableId);
        grantRandomItemToUser.parameters.Add("Admin Username");
        grantRandomItemToUser.parameters.Add("Password");
        grantRandomItemToUser.parameters.Add("Username Receiving Item");
        grantRandomItemToUser.parameters.Add("Drop Table Id");
        apiDictionary.Add("Grant user random item", grantRandomItemToUser);

        FunctionCaller purchaseItem = new FunctionCaller();
        purchaseItem.function4Params = 
            (username, password, itemId, price) => 
            playerInventoryController.PurchaseItem(username, password, itemId, price);
        purchaseItem.parameters.Add("Username");
        purchaseItem.parameters.Add("Password");
        purchaseItem.parameters.Add("Item Id");
        purchaseItem.parameters.Add("Price");
        apiDictionary.Add("Purchase item", purchaseItem);

        FunctionCaller getInventory = new FunctionCaller();
        getInventory.function2Params = (username, password) => playerInventoryController.GetInventory(username, password);
        getInventory.parameters.Add("Username");
        getInventory.parameters.Add("Password");
        apiDictionary.Add("Get inventory", getInventory);

        FunctionCaller consumeItem = new FunctionCaller();
        consumeItem.function3Params = (username, password, itemId) => playerInventoryController.ConsumeItem(username, password, itemId);
        consumeItem.parameters.Add("Username");
        consumeItem.parameters.Add("Password");
        consumeItem.parameters.Add("ItemId");
        apiDictionary.Add("Consume item", consumeItem);
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
        string param1, param2, param3, param4;

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
            case 4:
                param1 = parameters.transform.GetChild(0).Find("Text").GetComponent<Text>().text;
                param2 = parameters.transform.GetChild(1).Find("Text").GetComponent<Text>().text;
                param3 = parameters.transform.GetChild(2).Find("Text").GetComponent<Text>().text;
                param4 = parameters.transform.GetChild(3).Find("Text").GetComponent<Text>().text;
                function.function4Params.Invoke(param1, param2, param3, param4);
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

    public void UpdateScore()
    {
        List<PowerUp> powerUps = dropTableController.GetPowerUps();
        powerUps.RemoveAll(powerUp => DateTime.Now.ToUniversalTime() >= powerUp.expirationDateTime);
        PowerUp scoreMultiplierPowerUp = powerUps.FirstOrDefault();
        scoreMultiplier = scoreMultiplierPowerUp == null ? 1 : scoreMultiplierPowerUp.multiplierAmount;
        score = score + scoreMultiplier * 1;
        scoreText.text = "Score: " + score;
        Debug.Log("Score multiplier: " + scoreMultiplier + "x, " + scoreText.text);
    }
}
