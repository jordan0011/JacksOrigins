using UnityEngine;
using Photon.Bolt;
using System.Collections;

public class PlayerWeapons : MonoBehaviour
{
    [SerializeField]
    public GameObject RightHand;
    public int prefabId;

    [SerializeField]
    public GameObject BackShoulder;
    public int prefabid2;

    [SerializeField]
    public int testnumber = 10;
    [SerializeField]
    public bool testbool = false;
    public void Update()
    {
        if (testbool)
        {
            testbool = false;
            DeleteWeapon(0);
            UpdateWeapon(testnumber, 0);
        }
    }

    public void UpdateWeapon(int k, int a)
    {
        if (a == 0)
        {
            int childs = RightHand.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(RightHand.transform.GetChild(i).gameObject);
            }
            prefabId = k;
            GameObject weapon;
            weapon = Instantiate(ItemList.Items.itemPrefabs[k], RightHand.transform);
            weapon.transform.SetParent(RightHand.transform);
        }
        if (a == 1)
        {
            int childs = BackShoulder.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(BackShoulder.transform.GetChild(i).gameObject);
            }
            prefabid2 = k;
            GameObject backweapon;
            backweapon = Instantiate(ItemList.Items.itemPrefabs[k], BackShoulder.transform);
            backweapon.transform.SetParent(BackShoulder.transform);
        }
    }
    public void DeleteWeapon(int a)
    {
        if (a == 0)
        {

            int childs = RightHand.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(RightHand.transform.GetChild(i).gameObject);
            }
        }
        if (a == 1)
        {
            int childs = BackShoulder.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(BackShoulder.transform.GetChild(i).gameObject);
            }
        }
    }
    public void OnEnable()
    {
        int childs = RightHand.transform.childCount;
        for (int i = childs - 1; i > 0; i--)
        {
            Destroy(RightHand.transform.GetChild(i).gameObject);
        }
        if (prefabId != -1)
        {
            GameObject weapon;
            weapon = Instantiate(ItemList.Items.itemPrefabs[prefabId], RightHand.transform);
            weapon.transform.SetParent(RightHand.transform);
        }

        int childs1 = BackShoulder.transform.childCount;
        for (int i = childs1 - 1; i > 0; i--)
        {
            Destroy(BackShoulder.transform.GetChild(i).gameObject);
        }
        if (prefabid2 != -1)
        {
            GameObject backweapon;
            backweapon = Instantiate(ItemList.Items.itemPrefabs[prefabid2], BackShoulder.transform);
            backweapon.transform.SetParent(BackShoulder.transform);
        }
    }
}
