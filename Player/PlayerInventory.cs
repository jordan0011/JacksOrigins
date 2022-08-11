using UnityEngine;
using Photon.Bolt;
using System.Collections;

public class PlayerInventory : EntityBehaviour<INewPlayerState>
{
    item[] inventory = new item[8];
    item[] equipment = new item[2];    //private int[] startUpgrades = { -1, -1, -1, -1 };

    private PowerStats _powerStats;
    public item[] GetEquipment()
    {
        return equipment;
    }
    public item[] GetInventory()
    {
        return inventory;
    }
    public void Awake()
    {
        if (BoltNetwork.IsServer)
        {
            int[] startUpgrades = { -1, -1, -1, -1 };

            for (int i = 0; i < inventory.Length; i++)
            {
                inventory[i] = new item(-1, "empty", startUpgrades);
            }
            int[] starteq = { -1, -1, -1, -1 };

            for(int i =0; i<equipment.Length; i++)
            {
                equipment[i] = new item(-1, "empty", starteq);
            }

        }

        _powerStats = GetComponent<PowerStats>();

    }
    public void UpdateInventoryViaServer()
    {
        if (!BoltNetwork.IsServer)
        {

            if (entity.IsAttached)
            { 
                int[] startUpgrades = { -1, -1, -1, -1 };

                for (int i = 0; i < state.Items.Length; i++)
                {
                    if (state.Items[i].itemID == -1)
                    {
                        inventory[i] = new item(-1, "empty", startUpgrades);
                    }
                    else
                    {
                        int[] startUpgrades1 = { state.Items[i].Upgrade[0], state.Items[i].Upgrade[1], state.Items[i].Upgrade[2], state.Items[i].Upgrade[3] };
                        item temp = ItemList.Items.Allitems[state.Items[i].itemID];
                        inventory[i] = new item(temp.GetItemID(), temp.GetName(), startUpgrades1);
                    }
                }
                Debug.Log("Searching in inventory");
                //_powerStats.CalculateStats();
                GUI_Controller.Current.UpdateInventoryView(inventory);
            }
        }
    }

    public void UpdateEquipmentViaServer()
    {
        if (!BoltNetwork.IsServer)
        { 
            if (entity.IsAttached)
            {
                int[] startUpgrades = { -1, -1, -1, -1 };

                for (int i = 0; i < state.equiped.Length; i++)
                {
                    if (state.equiped[i].itemID == -1)
                    {
                        equipment[i] = new item(-1, "empty", startUpgrades);
                    }
                    else
                    {
                        int[] startUpgrade1 = { state.equiped[i].Upgrade[0], state.equiped[i].Upgrade[1], state.equiped[i].Upgrade[2], state.equiped[i].Upgrade[3] };
                        item temp = ItemList.Items.Allitems[state.equiped[i].itemID];
                        equipment[i] = new item(temp.GetItemID(), temp.GetName(), startUpgrade1);
                    }
                }
                Debug.Log("Searching in equipment");
                GUI_Controller.Current.UpdateEquipmentView(equipment);
            }
        }
    }

    public void InventoryAlteration(int alttype, int int1, int int2)
    {
        if (BoltNetwork.IsServer)
        {
            if (alttype == 0) // add item
            {
                    int pos = -1;
                    for (int i = 0; i < inventory.Length; i++)
                    {
                        if (inventory[i].GetItemID() == -1 && pos == -1)
                            pos = i;
                    }

                    if (pos > -1)
                    {
                        int[] emptySlotUpgrades = { -1, -1, -1, -1 };
                        item temp = ItemList.Items.Allitems[int1];
                        inventory[pos] = new item(temp.GetItemID(), temp.GetName(), emptySlotUpgrades);
                    }
            }
            else if (alttype == 1) // remove item
            {
                    state.Items[int1].itemID = -1;

                    int[] emptySlotUpgrades = { -1, -1, -1, -1 };
                    item temp = new item(-1, "empty", emptySlotUpgrades);
                    inventory[int1] = temp;
                    //GetComponent<playerInventory>().Delete
            }
            else if (alttype == 2) // switch pos 
            {

                int position1 = int1;
                int position2 = int2;
                int seconditemid;
                int[] seconditemslots = new int[4];
               
                if(inventory[position1].GetItemID() > -1 && position1 >=0 && position1 < inventory.Length && position2 >=0 && position2 < inventory.Length)
                {
                    if(inventory[position2].GetItemID() > -1)
                    {
                        seconditemid = inventory[position2].GetItemID();
                        for (int i = 0; i < inventory[position2].GetUpgradesArray().Length; i++)
                        {
                            int up;
                            up = inventory[position2].GetUpgradeSlot(i);
                            seconditemslots[i] = up;
                        }

                        int[] emptySlotUpgrades = { -1, -1, -1, -1 };
                        for(int k = 0; k< inventory[position2].GetUpgradesArray().Length; k++)
                        {
                            int up;
                            up = inventory[position1].GetUpgradeSlot(k);
                            emptySlotUpgrades[k] = up;
                        }
                        item temp = ItemList.Items.Allitems[inventory[position1].GetItemID()];
                        inventory[position2] = new item(temp.GetItemID(), temp.GetName(), emptySlotUpgrades);


                        
                        item temp1 = ItemList.Items.Allitems[seconditemid];
                        inventory[position1] = new item(temp1.GetItemID(), temp1.GetName(), seconditemslots);
                    }
                    else
                    {
                        int[] emptySlotUpgrades = { -1, -1, -1, -1 };
                        for (int k = 0; k < inventory[position1].GetUpgradesArray().Length; k++)
                        {
                            int up;
                            up = inventory[position1].GetUpgradeSlot(k);
                            emptySlotUpgrades[k] = up;
                        }
                        item temp = ItemList.Items.Allitems[inventory[position1].GetItemID()];
                        inventory[position2] = new item(temp.GetItemID(), temp.GetName(), emptySlotUpgrades);

                        int[] emptySlotUpgrades1 = { -1, -1, -1, -1 };
                        item temp1 = new item(-1, "empty", emptySlotUpgrades1);
                        inventory[position1] = temp1;
                    }
                }
            }
            else if (alttype == 3) // addUpgradeSlot
            {
                int pos = -1;
                int itemid = inventory[int1].GetItemID();
                for (int i = 0; i < inventory[int1].GetUpgradesArray().Length; i++)
                {
                    if (inventory[int1].GetUpgradeSlot(i) == -1 && pos == -1)
                        pos = i;
                }

                if (pos > -1)
                {
                    int[] emptySlotUpgrades = { -1, -1, -1, -1 };
                    emptySlotUpgrades[pos] = 0;
                    item temp = ItemList.Items.Allitems[itemid];
                    inventory[int1] = new item(temp.GetItemID(), temp.GetName(), emptySlotUpgrades);
                }
            }
            else if (alttype == 4) // addUpgrade
            {

            }
            else if (alttype == 5) // equip Item
            {
                int invfrom = int1;
                int equipPos = int2;

                if(inventory[invfrom].GetItemID() != -1 && equipment[equipPos].GetItemID() == -1)
                {
                    itemCategory category = ItemList.Items.itemInfo[inventory[invfrom].GetItemID()].getCategory();
                    if (category == itemCategory.Weapon)
                    {
                        int[] startUpgrade1 = { -1, -1, -1, -1 };
                        for(int h=0; h<inventory[invfrom].GetUpgradesArray().Length; h++)
                        {
                            int up;
                            up =inventory[invfrom].GetUpgradeSlot(h);
                            startUpgrade1[h] = up;
                        }
                        item temp = ItemList.Items.Allitems[inventory[invfrom].GetItemID()];
                        equipment[equipPos] = new item(temp.GetItemID(), temp.GetName(), startUpgrade1);

                        int[] startUpgrade2 = { -1, -1, -1, -1 };
                        inventory[invfrom] = new item(-1, "empty", startUpgrade2);
                    }
                }
                if (inventory[invfrom].GetItemID() != -1 && equipment[equipPos].GetItemID() > -1)
                {
                    itemCategory category = ItemList.Items.itemInfo[state.Items[invfrom].itemID].getCategory();
                    if (category == itemCategory.Weapon)
                    {
                        int tempitemid;
                        int[] tempitemslots = new int[4];

                        tempitemid = equipment[equipPos].GetItemID();
                        for(int k =0; k < tempitemslots.Length; k++)
                        {
                            int up;
                            up = equipment[equipPos].GetUpgradeSlot(k);
                            tempitemslots[k] = up;
                        }

                        int[] startUpgrades1 = { -1, -1, -1, -1 };
                        item temp1 = ItemList.Items.Allitems[inventory[invfrom].GetItemID()];
                        for(int k=0; k < startUpgrades1.Length; k++)
                        {
                            int up;
                            up = inventory[invfrom].GetUpgradeSlot(k);
                            startUpgrades1[k] = up;
                        }
                        equipment[equipPos] = new item(temp1.GetItemID(), temp1.GetName(), startUpgrades1);


                        item temp2 = ItemList.Items.Allitems[tempitemid];
                        inventory[invfrom] = new item(temp2.GetItemID(), temp2.GetName(), tempitemslots);

                    }
                }
            }
            else if(alttype == 6)
            {
                int equipfrom = int1;

                int pos = -1;
                for (int i = 0; i < inventory.Length; i++)
                {
                    if (inventory[i].GetItemID() == -1 && pos == -1)
                        pos = i;
                }

                if (pos > -1)
                {
                    if (equipment[equipfrom].GetItemID() != -1)
                    {

                        int[] startUpgrades = { -1, -1, -1, -1 };
                        item temp = ItemList.Items.Allitems[equipment[equipfrom].GetItemID()];
                        for(int k =0; k< startUpgrades.Length; k++)
                        {
                            int up =0;
                            up = equipment[equipfrom].GetUpgradeSlot(k);
                            startUpgrades[k] = up;
                        }
                        inventory[pos] = new item(temp.GetItemID(), temp.GetName(), startUpgrades);

                        int[] startUpgrades2 = { -1, -1, -1, -1 };
                        equipment[equipfrom] = new item(-1, "empty", startUpgrades2);
                    }
                }
            }
            else if (alttype == 7)
            {
                int pos = -1;
                for (int i = 0; i < equipment.Length; i++)
                {
                    if (equipment[i].GetItemID() == -1 && pos == -1)
                        pos = i;
                }

                if (pos > -1)
                {
                    int[] emptySlotUpgrades = { -1, -1, -1, -1 };
                    item temp = ItemList.Items.Allitems[int1];
                    equipment[pos] = new item(temp.GetItemID(), temp.GetName(), emptySlotUpgrades);
                }
            }
            SendInventoryInfo();
        }
    }
    public void SendInventoryInfo()
    {
        if (BoltNetwork.IsServer)
        {
            if (entity.IsAttached)
            {
                for (int k = 0; k < state.Items.Length; k++)
                {
                    state.Items[k].itemID = inventory[k].GetItemID();
                    for (int h = 0; h < state.Items[k].Upgrade.Length; h++)
                        state.Items[k].Upgrade[h] = inventory[k].GetUpgradeSlot(h);
                }

                for (int k = 0; k < state.equiped.Length; k++)
                {
                    state.equiped[k].itemID = equipment[k].GetItemID();
                    for (int h = 0; h < state.equiped[k].Upgrade.Length; h++)
                        state.equiped[k].Upgrade[h] = equipment[k].GetUpgradeSlot(h);
                }
            }
        }
        Debug.Log("Sending Inventory Info...");

        _powerStats.CalculateStats(equipment);
        GUI_Controller.Current.UpdateEquipmentView(equipment);
    }
    public void DeleteItem(int n)
    {
        int[] startUpgrades = { -1, -1, -1, -1 };
        inventory[n] = new item(-1, "empty", startUpgrades);

        GUI_Controller.Current.UpdateInventoryView(inventory);

    }
}
public class item
{
    private int itemID = 0;
    private string name = "";
    private int[] upgrades = { -1, -1, -1, -1 };

    public item(int MyID, string myName, int[] myUpgrades)
    {
        itemID = MyID;
        name = myName;
        upgrades = myUpgrades;
    }
    public int GetItemID()
    {
        return itemID;
    }
    public string GetName()
    {
        return name;
    }
    public int GetUpgradeSlot(int n)
    {
        return upgrades[n];
    }
    public int[] GetUpgradesArray()
    {
        return upgrades;
    }
}