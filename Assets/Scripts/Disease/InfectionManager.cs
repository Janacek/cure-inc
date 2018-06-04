using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionManager : MonoBehaviour
{
    public float TimeToNextWave = 1.0f;
    public float TimeToNextHeal = 0.7f;

    [HideInInspector]
    public int daysPassed = 0;
    int hoursPassed = 0;

    void Start()
    {
        hm = GameObject.Find("HexagonManager").GetComponent<HexagonManager>();

        Debug.Log(hm.Towns.Count);
        Group group = hm.Towns[Random.Range(0, hm.Towns.Count)];
        while (group.Buildings.Count < 3)
        {
            group = hm.Towns[Random.Range(0, hm.Towns.Count)];
        }
        GameObject city = group.Buildings[Random.Range(0, group.Buildings.Count)];
        Infos infos = city.GetComponent<Infos>();
        Camera.main.gameObject.GetComponent<CameraManager>().ZoomOnHexagon(city.GetComponent<Hexagon>());
        infectedTiles.Add(infos);
    }

    float timer = 0.0f;

    float timerHeal = 0.0f;

    float timerHourse = 0.0f;
    float sinceBeggining = 0.0f;

    void Update()
    {
        timer += Time.deltaTime * hm.Pause * hm.Accel * hm.GameOver;
        timerHourse += Time.deltaTime * hm.Pause * hm.Accel * hm.GameOver;
        sinceBeggining += Time.deltaTime * hm.Pause * hm.Accel * hm.GameOver;

        timerHeal += Time.deltaTime * hm.Pause * hm.Accel * hm.GameOver;

        if (timerHeal > TimeToNextHeal)
        {
            timerHeal = 0.0f;
            hm.Cash += 2;
            hm.Heal();
        }

        if (timerHourse > TimeToNextWave / 24)
        {
            timerHourse = 0.0f;
            ++hoursPassed;
            if (hoursPassed > 23)
                hoursPassed = 0;

            //hm.UI.HoursePassedText.text = "" + hoursPassed;
        }

        if (timer > TimeToNextWave)
        {
            timer = 0.0f;
            daysPassed++;

            infectedTiles.ForEach(t =>
            {
                hm.UI.DaysPassedText.text = "" + daysPassed;
                t.DaysPassed++;

                //if (t.InfectedPopulation <= 0 && t.DaysPassed > 30)
                //{
                //    infectedTiles.Remove(t);
                //}
                //else
                {
                    if (t.InfectedPopulation + t.DisinfectedPopulation + t.UndeadPopulation >= t.LivingPopulation)
                    {

                    }
                    else
                    {
                        // a(b²)+c
                        t.InfectedPopulation = (int)(0.9f * (Mathf.Pow(1.1f, t.DaysPassed + 10)) + 0.33f);
                    }
                    if (t.InfectedPopulation > t.LivingPopulation - t.DisinfectedPopulation - t.UndeadPopulation)
                    {
                        t.InfectedPopulation = t.LivingPopulation - t.DisinfectedPopulation - t.UndeadPopulation;
                    }

                    if (t.InfectedPopulation > t.OriginalPopulation / 2)
                    {
                        for (int ii = 0; ii < 2; ++ii)
                        {
                            int dir = Random.Range(0, 6);
                            GameObject n = hm.GetNeighbor(t.gameObject, (HexagonManager.EDirectionHexagon)dir);
                            if (n && n.GetComponent<Infos>().type != Infos.buildingType.Stopper &&
                        n.GetComponent<Infos>().tType != Infos.tileType.Mountain &&
                        n.GetComponent<Infos>().tType != Infos.tileType.Water)
                            {
                                Infos i = n.GetComponent<Infos>();
                                if (infectedTiles.Contains(i) == false)
                                    infectedTiles.Add(i);
                                i.InfectedPopulation += (t.InfectedPopulation * 5) / 100;
                                if (i.InfectedPopulation > i.LivingPopulation)
                                {
                                    i.InfectedPopulation = i.LivingPopulation;
                                }
                            }
                            else if (n && n.GetComponent<Infos>().type == Infos.buildingType.Stopper)
                            {
                                n.GetComponent<Infos>().life -= 5;

                                if (n.GetComponent<Infos>().life <= 0)
                                {
                                    hm.sfx.PlayerOnShot(2);
                                    hm.buildings.Remove(n);
                                    Destroy(n.GetComponent<Infos>().lifeBar.transform.parent.gameObject);
                                    Destroy(n.GetComponent<Infos>().model);
                                    n.GetComponent<Hexagon>().building = null;
                                    n.GetComponent<Infos>().Init();
                                }
                            }
                        }
                    }
                    else if (t.UndeadPopulation > 0)
                    {
                        int dir = Random.Range(0, 6);
                        GameObject n = hm.GetNeighbor(t.gameObject, (HexagonManager.EDirectionHexagon)dir);
                        if (n && n.GetComponent<Infos>().type != Infos.buildingType.Stopper &&
                        n.GetComponent<Infos>().tType != Infos.tileType.Mountain &&
                        n.GetComponent<Infos>().tType != Infos.tileType.Water)
                        {
                            Infos i = n.GetComponent<Infos>();
                            if (infectedTiles.Contains(i) == false)
                                infectedTiles.Add(i);
                            int aaa = (t.UndeadPopulation * 5) / 100;

                            if (t.LivingPopulation - aaa < 0)
                            {
                                aaa = t.LivingPopulation;
                            }

                            i.UndeadPopulation += aaa;
                            i.LivingPopulation += aaa;

                            t.LivingPopulation -= aaa;
                            t.UndeadPopulation -= aaa;
                            //if (i.UndeadPopulation > i.LivingPopulation)
                            //{
                            //    i.UndeadPopulation = i.LivingPopulation;
                            //}
                        }
                        else if (n && n.GetComponent<Infos>().type == Infos.buildingType.Stopper)
                        {
                            n.GetComponent<Infos>().life -= 10;

                            if (n.GetComponent<Infos>().life <= 0)
                            {
                                hm.buildings.Remove(n);
                                Destroy(n.GetComponent<Infos>().lifeBar.transform.parent.gameObject);
                                Destroy(n.GetComponent<Infos>().model);
                                n.GetComponent<Hexagon>().building = null;
                                n.GetComponent<Infos>().Init();
                            }
                        }
                    }

                    // ZOMBIES
                    if (t.DaysPassed > 20)
                    {
                        int nbrOfNewZombies = (t.InfectedPopulation * 1) / 100 + 1;
                        if (t.InfectedPopulation - nbrOfNewZombies < 0)
                        {
                            nbrOfNewZombies = t.InfectedPopulation;
                        }
                        t.InfectedPopulation -= nbrOfNewZombies;
                        t.UndeadPopulation += nbrOfNewZombies;
                    }
                }
            });
        }
    }

    List<Infos> infectedTiles = new List<Infos>();

    HexagonManager hm = null;
}
