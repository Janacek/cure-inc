using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreator : MonoBehaviour
{
    public enum ECharacterType
    {
        Medic,
        Soldier,
        Count
    }

    private GameObject medic = null;
    private GameObject soldier = null;
    [HideInInspector]
    public List<GameObject> medicInGame = new List<GameObject>();
    public List<GameObject> soldierInGame = new List<GameObject>();
    public Vector2 localOffsetBegin;
    public Vector2 localOffsetEnd;

    // Use this for initialization
    void Start()
    {
        medic = Resources.Load("medic") as GameObject;
        soldier = Resources.Load("medic") as GameObject;
    }

    public void PlaceCharacterOnHexagon(int numberOfPelos, Hexagon hexagon, ECharacterType characterType)
    {
        for (int i = 0; i < numberOfPelos; ++i)
        {
            Vector2 pos = new Vector2();
            pos.x = Random.Range(hexagon.transform.position.x + localOffsetBegin.x, hexagon.transform.position.x + localOffsetEnd.x);
            pos.y = Random.Range(hexagon.transform.position.y + localOffsetBegin.y, hexagon.transform.position.y + localOffsetEnd.y);
            if (characterType == ECharacterType.Medic)
            {
                GameObject medicGO = GameObject.Instantiate(medic);
                medicGO.transform.position = pos;
                medicGO.GetComponent<CharacterControl>().OnCurrentHexagonChanged(hexagon);
                medicGO.GetComponent<CharacterControl>().HexagonManager = GetComponent<HexagonManager>();
                medicInGame.Add(medicGO);
            }
            if (characterType == ECharacterType.Soldier)
            {
                GameObject soldierGO = GameObject.Instantiate(soldier);
                soldierGO.transform.position = pos;
                soldierGO.GetComponent<CharacterControl>().OnCurrentHexagonChanged(hexagon);
                soldierGO.GetComponent<CharacterControl>().HexagonManager = GetComponent<HexagonManager>();
                medicInGame.Add(soldierGO);
            }

        }
    }
}
