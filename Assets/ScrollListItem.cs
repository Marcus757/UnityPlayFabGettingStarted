using PlayFab;
using PlayFab.AdminModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PlayFab.GroupsModels;

public class ScrollListItem : MonoBehaviour
{
    public string displayText;
    public PlayFab.GroupsModels.EntityKey entityKey;
    public List<PlayerProfile> allPlayerProfiles;

    public void OnClickScrollListItem()
    {
        if (this.transform.parent.parent.parent.name != "ExistingGroupsView")
            return;

        GetAllSegments();
    }

    private void GetAllSegments()
    {
        PlayFabAdminAPI.GetAllSegments(new GetAllSegmentsRequest(), OnGetAllSegmentsSuccess, OnSharedError);
    }

    private void OnGetAllSegmentsSuccess(PlayFab.AdminModels.GetAllSegmentsResult result)
    {
        var segment = result.Segments.Find(x => x.Name == "All Players");
        if (segment != null)
        {
            var request = new GetPlayersInSegmentRequest { SegmentId = segment.Id };
            PlayFabAdminAPI.GetPlayersInSegment(request, OnGetAllPlayersSuccess, OnSharedError);
        }
    }

    private void OnGetAllPlayersSuccess(GetPlayersInSegmentResult result)
    {
        allPlayerProfiles = result.PlayerProfiles;

        var request = new ListGroupMembersRequest() { Group = entityKey };
        PlayFabGroupsAPI.ListGroupMembers(request, OnListGroupMembersSuccess, OnSharedError);
    }

    private void OnListGroupMembersSuccess(ListGroupMembersResponse response)
    {
        var playerIdsInGroup = new List<string>();
        response.Members.ForEach(role => { role.Members.ForEach(member => 
        {
            playerIdsInGroup.Add(member.Lineage["master_player_account"].Id);
        });});
        
        Groups groups = GameObject.Find("GamePanel").GetComponent<Groups>();
        groups.playersToAdd.itemList = new List<ScrollListItem>();

        var availablePlayerIdsToAdd = allPlayerProfiles.Where(playerProfile => !playerIdsInGroup.Contains(playerProfile.PlayerId)).ToList();
        availablePlayerIdsToAdd.OrderBy(player => player.DisplayName).ToList().ForEach(player =>
        {
            var scrollListItem = new ScrollListItem() { displayText = player.PlayerId };
            groups.playersToAdd.itemList.Add(scrollListItem);
        });

        groups.playersToAdd.AddItems();
    }

    private void OnSharedError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
}
