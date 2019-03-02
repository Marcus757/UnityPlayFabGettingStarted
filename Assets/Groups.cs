using PlayFab;
using PlayFab.GroupsModels;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using System.Linq;

public class Groups : MonoBehaviour
{
    public InputField groupNameInputField;
    public ScrollList existingGroups;
    public ScrollList playersToAdd;
    public ScrollList pendingInvitations;
    public List<PlayFab.ServerModels.PlayerProfile> allPlayerProfiles;

    //private readonly HashSet<KeyValuePair<string, string>> entityGroupPairs = new HashSet<KeyValuePair<string, string>>();
    //private readonly Dictionary<string, string> groupNameById = new Dictionary<string, string>();

    public void Start()
    {
        ListGroups(new EntityKey { Id = GameController.getEntityId(), Type = GameController.getEntityType() });
    }

    public void CreateGroup(string groupName, EntityKey entityKey)
    {
        var request = new CreateGroupRequest { GroupName = groupName, Entity = entityKey };
        PlayFabGroupsAPI.CreateGroup(request, OnCreateGroup, OnSharedError);
    }

    private void OnCreateGroup(CreateGroupResponse response)
    {
        Debug.Log("Group Created: " + response.GroupName + " -  " + response.Group.Id);

        var prevRequest = (CreateGroupRequest)response.Request;
        //entityGroupPairs.Add(new KeyValuePair<string, string>(prevRequest.Entity.Id, response.Group.Id));
        //groupNameById[response.Group.Id] = response.GroupName;

        ListGroups(new EntityKey { Id = GameController.getEntityId(), Type = GameController.getEntityType() });
    }

    private void OnSharedError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    public void ListGroups(EntityKey entityKey)
    {
        var request = new ListMembershipRequest { Entity = entityKey };
        PlayFabGroupsAPI.ListMembership(request, OnListMembership, OnSharedError);
    }

    private void OnListMembership(ListMembershipResponse response)
    {
        UpdateExisitingGroupsView(response.Groups);
    }

    public void UpdateExisitingGroupsView(List<GroupWithRoles> groups)
    {
        existingGroups.itemList = new List<ScrollListItem>();

        groups.OrderBy(group => group.GroupName).ToList().ForEach(group =>
        {
            var scrollListItem = new ScrollListItem() { displayText = group.GroupName, entityKey = group.Group };
            existingGroups.itemList.Add(scrollListItem);
        });

        existingGroups.AddItems();
    }

    public void UpdatePlayersToAddView(List<EntityMemberRole> members, List<PlayFab.ServerModels.PlayerProfile> allPlayerProfiles)
    {
        var playerEntityKeys = new List<EntityKey>();
        //members.ForEach(role => {
        //    role.Members.ForEach(member =>
        //    {
        //        playerEntityKeys.Add(member.Key);
        //    });
        //});

        members.ForEach(role =>
        {
            role.Members.ForEach(member =>
            {
                playerEntityKeys.Add(member.Lineage["master_player_account"]);
            });
        });

        playersToAdd.itemList = new List<ScrollListItem>();

        var availablePlayerIdsToAdd = allPlayerProfiles.Where(playerProfile => 
            !playerEntityKeys.Select(entityKeys => entityKeys.Id).Contains(playerProfile.PlayerId)).ToList();
        availablePlayerIdsToAdd.OrderBy(player => player.DisplayName).ToList().ForEach(player =>
        {
            var scrollListItem = new ScrollListItem()
            {
                displayText = string.IsNullOrEmpty(player.DisplayName) ? player.PlayerId : player.DisplayName
        };
            playersToAdd.itemList.Add(scrollListItem);
        });

        playersToAdd.AddItems();
    }

    private ScrollListItem GetSelectedItemFromScrollList(string viewName)
    {
        ToggleGroup toggleGroup = GameObject.Find(viewName).transform.Find("Viewport").Find("Content").Find("ToggleGroup").GetComponent<ToggleGroup>();
        List<ScrollListItem> scrollListItems = new List<ScrollListItem>();

        foreach (Transform item in toggleGroup.transform)
            scrollListItems.Add(item.GetComponentInChildren<ScrollListItem>());

        return scrollListItems.Find(scrollListItem => scrollListItem.GetComponent<UnityEngine.UI.Toggle>().isOn);
    }

    public void RefreshPlayersToAddView()
    {
        PlayFabAdminAPI.GetAllSegments(new PlayFab.AdminModels.GetAllSegmentsRequest(), OnGetAllSegmentsSuccess, OnSharedError);
    }

    private void OnGetAllSegmentsSuccess(PlayFab.AdminModels.GetAllSegmentsResult result)
    {
        var segment = result.Segments.Find(x => x.Name == "All Players");
        if (segment != null)
        {
            var request = new PlayFab.ServerModels.GetPlayersInSegmentRequest { SegmentId = segment.Id };
            PlayFabServerAPI.GetPlayersInSegment(request, OnGetAllPlayersSuccess, OnSharedError);
        }
    }

    private void OnGetAllPlayersSuccess(PlayFab.ServerModels.GetPlayersInSegmentResult result)
    {
        allPlayerProfiles = result.PlayerProfiles;

        var selectedGroup = GetSelectedItemFromScrollList("ExistingGroupsView");
        var request = new ListGroupMembersRequest() { Group = selectedGroup.entityKey };
        PlayFabGroupsAPI.ListGroupMembers(request, OnListGroupMembersSuccess, OnSharedError);
    }

    private void OnListGroupMembersSuccess(ListGroupMembersResponse response)
    {
        Groups groups = GameObject.Find("GamePanel").GetComponent<Groups>();
        groups.UpdatePlayersToAddView(response.Members, allPlayerProfiles);
    }

    public void OnClickCreateGroup()
    {
        var entityKey = new PlayFab.GroupsModels.EntityKey { Id = GameController.getEntityId(), Type = GameController.getEntityType() };
        CreateGroup(groupNameInputField.text, entityKey);
    }

    public void OnClickAddPlayer()
    {
        var selectedPlayer = GetSelectedItemFromScrollList("PlayersView");
        var selectedGroup = GetSelectedItemFromScrollList("ExistingGroupsView");

        if (selectedPlayer && selectedGroup)
        {
            var request = new PlayFab.ServerModels.GetUserAccountInfoRequest() { PlayFabId = selectedPlayer.displayText };
            PlayFabServerAPI.GetUserAccountInfo(request, OnGetUserAccountInfoSuccess, OnSharedError);
        }
    }

    private void OnGetUserAccountInfoSuccess(PlayFab.ServerModels.GetUserAccountInfoResult result)
    {
        var selectedGroup = GetSelectedItemFromScrollList("ExistingGroupsView");
        var selectedPlayerEntityKey = result.UserInfo.TitleInfo.TitlePlayerAccount;

        var request = new InviteToGroupRequest()
        {
            Group = selectedGroup.entityKey, Entity = new EntityKey() { Id = selectedPlayerEntityKey.Id, Type = selectedPlayerEntityKey.Type }
        };
        PlayFabGroupsAPI.InviteToGroup(request, OnInviteToGroupSuccess, OnSharedError);
    }

    private void OnInviteToGroupSuccess(InviteToGroupResponse response)
    {
        RefreshPlayersToAddView();
    }

    public void OnClickAcceptInvitation()
    {

    }
}
