using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCreator : MonoBehaviour
{
    public enum EBuildingType
    {
        Caserne,
        Hospital,
        Stopper
    }

    private GameObject caserne = null;
    private GameObject hospital = null;
    private GameObject stopper = null;

    public Vector3 offset = new Vector3(0.8f, -1, 0);

    // Use this for initialization
    void Start ()
    {
        caserne = Resources.Load("Caserne") as GameObject;
        hospital = Resources.Load("Hospital") as GameObject;
        stopper = Resources.Load("Stopper") as GameObject;

    }

    public bool PlaceBuildingOnHexagon(Hexagon hexagon, EBuildingType buildingType)
    {
        if (buildingType == EBuildingType.Caserne && hexagon.GetComponent<Infos>().type == Infos.buildingType.None)
        {
            GameObject building = GameObject.Instantiate(caserne);
            building.transform.parent = hexagon.transform;
            building.transform.localPosition = offset;
            hexagon.GetComponent<Infos>().model = building;
            hexagon.GetComponent<Infos>().type = Infos.buildingType.Caserne;
            return true;
        }
        if (buildingType == EBuildingType.Hospital && hexagon.GetComponent<Infos>().type == Infos.buildingType.None)
        {
            GameObject building = GameObject.Instantiate(hospital);
            building.transform.parent = hexagon.transform;
            building.transform.localPosition = offset;
            hexagon.GetComponent<Infos>().model = building;
            hexagon.GetComponent<Infos>().type = Infos.buildingType.Hospital;
            return true;
        }
        if (buildingType == EBuildingType.Stopper && hexagon.GetComponent<Infos>().type == Infos.buildingType.None)
        {
            GameObject building = GameObject.Instantiate(stopper);
            building.transform.parent = hexagon.transform;
            building.transform.localPosition = offset;
            hexagon.GetComponent<Infos>().model = building;
            hexagon.GetComponent<Infos>().type = Infos.buildingType.Stopper;
            return true;
        }
        return false;
    }

    [HideInInspector]
    public bool isOverUI = false;
}
