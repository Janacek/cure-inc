using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Infos : MonoBehaviour
{
    public enum buildingType
    {
        None = 0,
        Village,
        Town,
        Caserne,
        Hospital,
        Stopper
    }

    public enum tileType
    {
        Water = 0,
        Sand,
        Grass,
        Mountain,
    }

    public int OriginalPopulation = 0;
    public int LivingPopulation = 0;
    public int DisinfectedPopulation = 0;
    public int InfectedPopulation = 0;
    public int DeadPopulation = 0;
    public int UndeadPopulation = 0;

    [HideInInspector]
    public int DaysPassed = 0;

    public float InfectionQuotient
    {
        get
        {
            if (OriginalPopulation <= 0)
                return 0;
            return ((float)InfectedPopulation + (float)UndeadPopulation) / (float)OriginalPopulation;

        }
    }

    public void Start()
    {
        h = GetComponent<Hexagon>();
    }

    public void Init()
    {
        life = 0;
        maxlife = 0;
        type = buildingType.None;
        inZoneBuildings.Clear();
        numberOfEntities = 0;
    }

    public void Update()
    {
        if (lifeBar && (numberOfEntities > 0 || type == buildingType.Stopper))
            lifeBar.fillAmount = (float)life / (float)maxlife;

        h.hm.numberOfZ += UndeadPopulation + DeadPopulation;
        h.hm.totalPop += OriginalPopulation;
        h.hm.numberOfInf += InfectedPopulation;
    }

    public buildingType type = buildingType.None;
    public tileType tType = tileType.Grass;
    public GameObject model;

    Hexagon h;

    public int numberOfEntities = 0;

    public int life = 0;
    public int maxlife = 0;

    public Image lifeBar;

    public List<GameObject> inZoneBuildings = new List<GameObject>();
}
