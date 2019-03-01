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
        existingGroups.itemList = new List<ScrollListItem>();

        response.Groups.OrderBy(group => group.GroupName).ToList().ForEach(group => 
        {
            var scrollListItem = new ScrollListItem() { displayText = group.GroupName, entityKey = group.Group };
            existingGroups.itemList.Add(scrollListItem);
        });

        existingGroups.AddItems();
    }

    public void OnClickCreateGroup()
    {
        var entityKey = new PlayFab.GroupsModels.EntityKey { Id = GameController.getEntityId(), Type = GameController.getEntityType() };
        CreateGroup(groupNameInputField.text, entityKey);
    }

}
