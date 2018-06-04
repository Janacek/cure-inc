using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierControl : CharacterControl
{
    public override void OnCurrentHexagonChanged(Hexagon newHexagon)
    {
        //if (CurrentHexagon)
        //{
        //    CurrentHexagon.gameObject.GetComponent<Infos>().soldierList.Remove(this);
        //}
        //newHexagon.gameObject.GetComponent<Infos>().soldierList.Add(this);
        //base.OnCurrentHexagonChanged(newHexagon);
    }
}
