using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class UIRaycastBlocker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler// required interface when using the OnPointerEnter method.
{
    public HexagonManager hm;

    //Do this when the cursor enters the rect area of this selectable UI object.
    public void OnPointerEnter(PointerEventData eventData)
    {
        hm.isOverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hm.isOverUI = false;
    }
}