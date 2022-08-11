using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlotScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private GameObject poweressence;
    [SerializeField]
    private Text petext;
    [SerializeField]
    private GameObject darkessence;
    [SerializeField]
    private Text detext;
    [SerializeField]
    private GameObject Icon;
    [SerializeField]
    private Text itemname;

    private int slotId;

    private ItemStorePanel itemstorepanel;


    public void OnPointerEnter(PointerEventData eventData)
    {
        itemstorepanel.ItemSlotMouseEnter(slotId);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        itemstorepanel.ItemSlotMouseExit(slotId);
    }

    public void SetFullStats(GameObject myicon, string myname, int power, int dark, int myslotid, ItemStorePanel myitemsystem)
    {
        itemname.text = myname;
        slotId = myslotid;
        itemstorepanel = myitemsystem;

        SetStats(power, dark);
    }
    public void SetStats(int power, int dark)
    {
        if(power== 0)
        {
            poweressence.SetActive(false);
        }
        else
        {
            poweressence.SetActive(true);
            petext.text = power.ToString();
        }

        if(dark == 0)
        {
            darkessence.SetActive(false);
        }
        else
        {
            darkessence.SetActive(true);
            detext.text = dark.ToString();
        }
    }
}
