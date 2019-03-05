using PlayFab;
using PlayFab.GroupsModels;

using UnityEngine;
using PlayFab.ClientModels;
using PlayFab.AuthenticationModels;
using EmptyResponse = PlayFab.GroupsModels.EmptyResponse;

public class GroupController : MonoBehaviour
{
    public void CreateGroup(string adminUsername, string password, string groupName)
    {
        var loginWithPlayFabRequest = new LoginWithPlayFabRequest { Username = adminUsername, Password = password };

        PlayFabClientAPI.LoginWithPlayFab(loginWithPlayFabRequest,
            delegate (LoginResult loginResult)
            {
                PlayFabAuthenticationAPI.GetEntityToken(new PlayFab.AuthenticationModels.GetEntityTokenRequest(),
                    delegate (GetEntityTokenResponse getEntityTokenResponse)
                    {
                        var createGroupRequest = new CreateGroupRequest {
                            GroupName = groupName,
                            Entity = ConvertEntityKey(getEntityTokenResponse.Entity)
                        };

                        PlayFabGroupsAPI.CreateGroup(createGroupRequest,
                            delegate (CreateGroupResponse response)
                            {
                                Debug.Log("Group was successfully created: " + response.GroupName + " -  " + response.Group.Id);
                            },
                            SharedError.OnSharedError);
                    },
                    SharedError.OnSharedError);
            },
            SharedError.OnSharedError);
    }

    public void InvitePlayerToGroup(string adminUsername, string password, string groupName, string usernameToAdd)
    {
        var loginWithPlayFabRequest = new LoginWithPlayFabRequest { Username = adminUsername, Password = password };

        PlayFabClientAPI.LoginWithPlayFab(loginWithPlayFabRequest,
            delegate (LoginResult loginResult)
            {
                var getGroupRequest = new GetGroupRequest() { GroupName = groupName };

                PlayFabGroupsAPI.GetGroup(getGroupRequest,
                    delegate (GetGroupResponse getGroupResponse)
                    {
                        var getAccountInfoRequest = new GetAccountInfoRequest() { Username = usernameToAdd };

                        PlayFabClientAPI.GetAccountInfo(getAccountInfoRequest,
                            delegate (GetAccountInfoResult getAccountInfoResult)
                            {
                                var inviteToGroupRequest = new InviteToGroupRequest()
                                {
                                    Group = getGroupResponse.Group,
                                    Entity = ConvertEntityKey(getAccountInfoResult.AccountInfo.TitleInfo.TitlePlayerAccount)
                                };

                                PlayFabGroupsAPI.InviteToGroup(inviteToGroupRequest,
                                    delegate (InviteToGroupResponse inviteToGroupResponse)
                                    {
                                        Debug.Log("Admin username: " + adminUsername +
                                            " successfully added username: " + usernameToAdd +
                                            " to group: " + groupName);
                                    },
                                    SharedError.OnSharedError);
                            },
                            SharedError.OnSharedError);
                    },
                    SharedError.OnSharedError);
            },
            SharedError.OnSharedError);
    }

    public void AcceptInvitationToGroup(string usernameToAccept, string password, string groupName)
    {
        var loginWithPlayFabRequest = new LoginWithPlayFabRequest { Username = usernameToAccept, Password = password };

        PlayFabClientAPI.LoginWithPlayFab(loginWithPlayFabRequest,
            delegate (LoginResult loginResult)
            {
                var getAccountInfoRequest = new GetAccountInfoRequest() { Username = usernameToAccept };

                PlayFabClientAPI.GetAccountInfo(getAccountInfoRequest,
                    delegate (GetAccountInfoResult getAccountInfoResult)
                    {
                        var getGroupRequest = new GetGroupRequest() { GroupName = groupName };

                        PlayFabGroupsAPI.GetGroup(getGroupRequest,
                            delegate (GetGroupResponse getGroupResponse)
                            {
                                var acceptGroupInvitationRequest = new AcceptGroupInvitationRequest()
                                {
                                    Entity = ConvertEntityKey(getAccountInfoResult.AccountInfo.TitleInfo.TitlePlayerAccount),
                                    Group = getGroupResponse.Group
                                };

                                PlayFabGroupsAPI.AcceptGroupInvitation(acceptGroupInvitationRequest,
                                    delegate (EmptyResponse emptyResponse)
                                    {
                                        Debug.Log("Username: " + usernameToAccept + " has accepted an invitation to group: " + groupName);
                                    },
                                    SharedError.OnSharedError);
                            },
                            SharedError.OnSharedError);
                    },
                    SharedError.OnSharedError);
            },
            SharedError.OnSharedError);
    }

    private PlayFab.GroupsModels.EntityKey ConvertEntityKey(PlayFab.AuthenticationModels.EntityKey entityKey)
    {
        return new PlayFab.GroupsModels.EntityKey() { Id = entityKey.Id, Type = entityKey.Type };
    }

    private PlayFab.GroupsModels.EntityKey ConvertEntityKey(PlayFab.ClientModels.EntityKey entityKey)
    {
        return new PlayFab.GroupsModels.EntityKey() { Id = entityKey.Id, Type = entityKey.Type };
    }
}
