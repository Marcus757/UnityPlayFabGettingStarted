using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollList : MonoBehaviour
{
    public List<ScrollListItem> itemList;
    public Transform contentPanel;
    public GameObject prefab;
    
    public void AddItems()
    {
        ClearItems();

        foreach (var item in itemList)
        {
            GameObject instance = GameObject.Instantiate(prefab);
            instance.transform.SetParent(contentPanel);
            Text text = instance.GetComponentInChildren<Text>();
            text.text = item.displayText;

            var scrollListItem = instance.GetComponent<ScrollListItem>();
            scrollListItem.displayText = item.displayText;
            scrollListItem.entityKey = item.entityKey;
        }
        
    }

    public void ClearItems()
    {
        foreach (Transform child in contentPanel.transform)
            Destroy(child.gameObject);
    }
}
