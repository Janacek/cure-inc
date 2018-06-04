using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private List<Hexagon> visitedHex = new List<Hexagon>();

    public List<Hexagon> ComputePathBetweenHexagons(Hexagon hexA, Hexagon hexB, HexagonManager map)
    {
        visitedHex.Clear();
        List<Hexagon> path = new List<Hexagon>();
        path.Add(hexA);
        RecursivePathComputing(ref path,hexA, hexA, hexB, map);


        return path;
    }

    private void RecursivePathComputing(ref List<Hexagon> path, Hexagon firtHex, Hexagon hexA, Hexagon hexB, HexagonManager map)
    {
        Vector3 start = hexA.transform.position;
        Vector3 end = hexB.transform.position;
        float maxDistance = Vector3.Distance(start, end) + map.sizeHexagon;

        float bestRatio = 0;
        Hexagon chosenHexagon = null;

        for (int i = 0; i < (int)HexagonManager.EDirectionHexagon.eCount; ++i)
        {
            GameObject neighbor = map.GetNeighbor(hexA.gameObject, (HexagonManager.EDirectionHexagon)i);
           
            if (neighbor && neighbor.GetComponent<Hexagon>() == hexB)
            {
                chosenHexagon = hexB;
                path.Add(chosenHexagon);
                return;
            }
            if (neighbor)
            {
                float distance = Vector3.Distance(neighbor.transform.position, end);
                float ratio = (maxDistance - distance) / maxDistance;
                ratio = Mathf.Abs(ratio);
                ratio = neighbor.GetComponent<Hexagon>().Weight * ratio;
             
                if (ratio > bestRatio && !visitedHex.Find(item => item == neighbor.GetComponent<Hexagon>()))
                {
                    bestRatio = ratio;
                    chosenHexagon = neighbor.GetComponent<Hexagon>();
                }

            }
        }

        if (chosenHexagon == null)
        {
            if (path.Count == 1)
            {
                Debug.Log("NOT PATH FOUND :/");
                return;
            }
            if (path[path.Count - 1] == hexB)
            {
                return;
            }
            path.RemoveAt(path.Count - 1);
            RecursivePathComputing(ref path, firtHex, path[path.Count -1], hexB, map);
        }

        if (chosenHexagon && chosenHexagon != hexB)
        {
            visitedHex.Add(chosenHexagon);
            path.Add(chosenHexagon);
            RecursivePathComputing(ref path, firtHex, chosenHexagon, hexB, map);
        }
    }
}
