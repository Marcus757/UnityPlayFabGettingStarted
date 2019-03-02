using PlayFab;
using PlayFab.AdminModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PlayFab.GroupsModels;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollListItem : MonoBehaviour
{
    public string displayText;
    public PlayFab.GroupsModels.EntityKey entityKey;
    //private BaseEventData m_BaseEvent;

    public void OnClickScrollListItem()
    {
        if (this.transform.parent.parent.parent.parent.name != "ExistingGroupsView" || !GetComponent<Toggle>().isOn)
            return;


        Groups groups = GameObject.Find("GamePanel").GetComponent<Groups>();
        groups.RefreshPlayersToAddView();
    }

    //public bool IsSelected()
    //{
    //    return IsHighlighted(m_BaseEvent);
    //}
}
