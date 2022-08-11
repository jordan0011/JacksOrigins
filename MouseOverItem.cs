using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseOverItem : MonoBehaviour
{
    [SerializeField]
    Text itemname;
    [SerializeField]
    GameObject itemimage;
    [SerializeField]
    Text itemAttributes;

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void SetPanel(string name, GameObject icon, string attributes)
    {
        itemname.fontSize = 42;
        if(name.Length >= 18)
        {
            itemname.fontSize = itemname.fontSize - (name.Length - 16);
        }

        itemname.text = name;
        itemAttributes.text = attributes;

        int i = 0;

        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[itemimage.transform.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in itemimage.transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            DestroyImmediate(child.gameObject);
        }


        GameObject sample = Instantiate(icon, transform);
        sample.transform.SetParent(itemimage.transform);
    }
}
