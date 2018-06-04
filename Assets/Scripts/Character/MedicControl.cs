using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicControl : CharacterControl
{
    public override void OnCurrentHexagonChanged(Hexagon newHexagon)
    {
        //if (CurrentHexagon)
        //{
        //    CurrentHexagon.gameObject.GetComponent<Infos>().medicsList.Remove(this);
        //}
        //newHexagon.gameObject.GetComponent<Infos>().medicsList.Add(this);
        //base.OnCurrentHexagonChanged(newHexagon);
    }
}
