using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityDetails : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject DetailPanel;
    public void OnPointerEnter(PointerEventData eventData)
    {
        DetailPanel.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        DetailPanel.SetActive(false);
    }
}
