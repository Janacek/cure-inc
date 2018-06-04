using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class HexagonManager : MonoBehaviour
{
    public int sizeMapX;
    public int sizeMapY;
    public float sizeHexagon;
    public Material Material;

    [HideInInspector]
    public SFX sfx;

    [HideInInspector]
    public int Pause = 1;

    [HideInInspector]
    public float Accel = 1;

    [HideInInspector]
    public float GameOver = 1;

    [HideInInspector]
    public List<Group> Towns = new List<Group>();

    [HideInInspector]
    public int Cash = 400;

    public static List<Color> Colors = new List<Color>()
    {
        Color.blue,
        Color.blue,
        Color.blue,
        Color.blue,
        Color.yellow,
        Color.yellow,
        Color.yellow,
        Color.green,
        Color.green,
        Color.green,
        Color.green,
        Color.green,
        Color.green,
        Color.green,
        Color.green,
        Color.green,
        Color.green,
        Color.grey,
        Color.grey,
        Color.grey,
    };

    public static List<Infos.tileType> TileTypes = new List<Infos.tileType>()
    {
        Infos.tileType.Water,
        Infos.tileType.Water,
        Infos.tileType.Water,
        Infos.tileType.Water,
        Infos.tileType.Sand,
        Infos.tileType.Sand,
        Infos.tileType.Sand,
        Infos.tileType.Grass,
        Infos.tileType.Grass,
        Infos.tileType.Grass,
        Infos.tileType.Grass,
        Infos.tileType.Grass,
        Infos.tileType.Grass,
        Infos.tileType.Grass,
        Infos.tileType.Grass,
        Infos.tileType.Grass,
        Infos.tileType.Grass,
        Infos.tileType.Mountain,
        Infos.tileType.Mountain,
        Infos.tileType.Mountain,
    };

    public enum EDirectionHexagon
    {
        eTopRight,
        eRight,
        eDownRight,
        eDownLeft,
        eLeft,
        eTopLeft,
        eCount
    }

    public Vector2Int[] dd =
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1),
    };

    private GameObject[][] hexagonMap;

    // Use this for initialization
    void Start()
    {
        System.DateTime now = System.DateTime.Now;
        Random.InitState(PlayerPrefs.GetInt("Seed", now.Hour + now.Millisecond));

        sfx = GameObject.Find("SFX").GetComponent<SFX>();

        UI = GameObject.Find("Canvas").GetComponent<UIManager>();
        bc = GetComponent<BuildingCreator>();
        town = Resources.Load("Town") as GameObject;
        village = Resources.Load("Village") as GameObject;
        hexagonMap = new GameObject[sizeMapX][];
        GameObject LinePref = Resources.Load("Line") as GameObject;
        zombie = Resources.Load("Zombie") as GameObject;

        lifeBar = Resources.Load("LifeBars") as GameObject;

        smallRing = Resources.Load("Ring") as GameObject;
        bigRing = Resources.Load("RingBig") as GameObject;

        for (int x = 0; x < sizeMapX; ++x)
        {
            hexagonMap[x] = new GameObject[sizeMapY /*+ sizeMapX / 2*/];

            for (int y = 0; y < sizeMapY; ++y)
            {
                float r = y % 2;
                GameObject hexagonGO = new GameObject();
                Hexagon h = hexagonGO.AddComponent<Hexagon>();
                h.ConstructHexagon(new Vector2((x * Mathf.Sqrt(3) * sizeHexagon) + ((Mathf.Sqrt(3) * sizeHexagon / 2) * y), (y * sizeHexagon * 2) * 0.75f), Material, LinePref);
                hexagonMap[x][y] = hexagonGO;
                h.x = x;
                h.y = y/* + x / 2*/;
                h.hm = this;
                h.Zombie = GameObject.Instantiate(zombie);
                h.Zombie.transform.parent = h.transform;
                h.Zombie.transform.localPosition = Vector3.zero + new Vector3(0, -0.5f, 0);
                h.Zombie.SetActive(false);
            }
        }
        //GetComponent<CharacterCreator>().PlaceCharacterOnHexagon(1, hexagonMap[0][0].GetComponent<Hexagon>(), CharacterCreator.ECharacterType.Medic);

        GenerateBuildings();
        CreateGroups();
        CreatePopulation();

        im = GameObject.Find("InfectionManager").GetComponent<InfectionManager>();
    }

    #region Population

    public Vector2Int PopNone;
    public Vector2Int PopTown;
    public Vector2Int PopVillage;

    void CreatePopulation()
    {
        for (int x = 0; x < sizeMapX; ++x)
        {
            for (int y = 0; y < sizeMapY; ++y)
            {
                Infos infos = hexagonMap[x][y].GetComponent<Infos>();
                if (infos.tType != Infos.tileType.Mountain && infos.tType != Infos.tileType.Water)
                {

                    if (infos.type == Infos.buildingType.None)
                        infos.LivingPopulation = Random.Range(PopNone.x, PopNone.y);
                    else if (infos.type == Infos.buildingType.Town)
                        infos.LivingPopulation = Random.Range(PopTown.x, PopTown.y);
                    else if (infos.type == Infos.buildingType.Village)
                        infos.LivingPopulation = Random.Range(PopVillage.x, PopTown.y);


                }
                else
                {
                    infos.LivingPopulation = 0;
                }

                infos.OriginalPopulation = infos.LivingPopulation;
            }
        }
    }

    #endregion

    #region Groups

    void CreateGroups()
    {
        List<GameObject> hexagonsWithBuilding = new List<GameObject>();

        for (int x = 0; x < sizeMapX; ++x)
        {
            for (int y = 0; y < sizeMapY; ++y)
            {
                Hexagon h = hexagonMap[x][y].GetComponent<Hexagon>();

                if (h.building)
                {
                    hexagonsWithBuilding.Add(h.gameObject);
                }
            }
        }

        while (hexagonsWithBuilding.Count > 0)
        {
            GameObject goh = hexagonsWithBuilding[0];
            Group newGroup = new Group();

            RecGetNeighborBuilding(ref hexagonsWithBuilding, goh, ref newGroup);

            // Complete group
            Towns.Add(newGroup);
        }

    }

    void RecGetNeighborBuilding(ref List<GameObject> buildings, GameObject currentBuilding, ref Group group)
    {
        currentBuilding.GetComponent<Hexagon>().checkedOn = true;
        buildings.Remove(currentBuilding);
        group.Buildings.Add(currentBuilding);

        GameObject n = GetNeighbor(currentBuilding, EDirectionHexagon.eTopLeft);
        if (n && n.GetComponent<Hexagon>().building != null && n.GetComponent<Hexagon>().checkedOn == false)
            RecGetNeighborBuilding(ref buildings, n, ref group);

        n = GetNeighbor(currentBuilding, EDirectionHexagon.eTopRight);
        if (n && n.GetComponent<Hexagon>().building != null && n.GetComponent<Hexagon>().checkedOn == false)
            RecGetNeighborBuilding(ref buildings, n, ref group);

        n = GetNeighbor(currentBuilding, EDirectionHexagon.eRight);
        if (n && n.GetComponent<Hexagon>().building != null && n.GetComponent<Hexagon>().checkedOn == false)
            RecGetNeighborBuilding(ref buildings, n, ref group);

        n = GetNeighbor(currentBuilding, EDirectionHexagon.eDownRight);
        if (n && n.GetComponent<Hexagon>().building != null && n.GetComponent<Hexagon>().checkedOn == false)
            RecGetNeighborBuilding(ref buildings, n, ref group);

        n = GetNeighbor(currentBuilding, EDirectionHexagon.eDownLeft);
        if (n && n.GetComponent<Hexagon>().building != null && n.GetComponent<Hexagon>().checkedOn == false)
            RecGetNeighborBuilding(ref buildings, n, ref group);

        n = GetNeighbor(currentBuilding, EDirectionHexagon.eLeft);
        if (n && n.GetComponent<Hexagon>().building != null && n.GetComponent<Hexagon>().checkedOn == false)
            RecGetNeighborBuilding(ref buildings, n, ref group);

    }
    #endregion

    #region GENERATE BUILDINGS

    Vector3 offset = new Vector3(0, 0, 0);
    GameObject town = null;
    GameObject village = null;

    void GenerateBuildings()
    {
        int numberOfBuildingsFirstPass = sizeMapX * sizeMapY / 100;
        for (int i = 0; i < numberOfBuildingsFirstPass; ++i)
        {
            Vector2 position = new Vector2(Random.Range(0, sizeMapX),
                Random.Range(0, sizeMapY));

            Hexagon h = hexagonMap[(int)position.x][(int)position.y].GetComponent<Hexagon>();

            if (h.GetComponent<Infos>().tType == Infos.tileType.Water ||
                h.GetComponent<Infos>().tType == Infos.tileType.Mountain ||
                h.building != null)
            {
                while (h.GetComponent<Infos>().tType == Infos.tileType.Water ||
                h.GetComponent<Infos>().tType == Infos.tileType.Mountain ||
                h.building != null)
                {
                    position = new Vector2(Random.Range(0, sizeMapX),
                Random.Range(0, sizeMapY));
                    h = hexagonMap[(int)position.x][(int)position.y].GetComponent<Hexagon>();
                }
            }

            hexagonMap[(int)position.x][(int)position.y].GetComponent<Infos>().type = Infos.buildingType.Town;
            int rnd = Random.Range(0, 2);
            h.building = rnd == 0 ? GameObject.Instantiate(town) : GameObject.Instantiate(village);
            h.building.transform.parent = h.transform;
            h.building.transform.localPosition = offset;
            h.GetComponent<Infos>().type = rnd == 0 ? Infos.buildingType.Town : Infos.buildingType.Village;
        }

        for (int i = 0; i < 3; ++i)
        {
            Pass();
        }
    }

    List<int> chanceToSpawn = new List<int>()
    {
        0,
        10,
        15,
        25,
        50,
        75,
        100,
    };

    void Pass()
    {
        for (int x = 0; x < sizeMapX; ++x)
        {
            for (int y = 0; y < sizeMapY; ++y)
            {
                Hexagon h = hexagonMap[x][y].GetComponent<Hexagon>();
                if (h.GetComponent<Infos>().tType != Infos.tileType.Mountain &&
                    h.GetComponent<Infos>().tType != Infos.tileType.Water &&
                    h.building == null)
                {

                    int nbrOfTowns = 0;
                    int nbrOfVillages = 0;
                    for (int i = 0; i < (int)EDirectionHexagon.eCount; ++i)
                    {
                        GameObject ngo = GetNeighbor(h.gameObject, (EDirectionHexagon)i);
                        if (ngo && ngo.GetComponent<Hexagon>().building && ngo.GetComponent<Infos>().type == Infos.buildingType.Town)
                        {
                            ++nbrOfTowns;
                        }
                        else if (ngo && ngo.GetComponent<Hexagon>().building && ngo.GetComponent<Infos>().type == Infos.buildingType.Village)
                        {
                            ++nbrOfVillages;
                        }
                    }

                    bool addBuilding = Random.Range(0, 100) <= chanceToSpawn[nbrOfVillages + nbrOfTowns] ? true : false;
                    if (addBuilding)
                    {
                        h.building = GameObject.Instantiate(nbrOfTowns >= nbrOfVillages ? town : village);
                        h.GetComponent<Infos>().type = nbrOfTowns >= nbrOfVillages ? Infos.buildingType.Town : Infos.buildingType.Village;
                        h.building.transform.parent = h.transform;
                        h.building.transform.localPosition = offset;
                    }
                }
            }
        }
    }

    #endregion

    public GameObject GetNeighbor(GameObject hexa, EDirectionHexagon eDirection)
    {
        Hexagon h = hexa.GetComponent<Hexagon>();
        if (h.x + dd[(int)eDirection].x < 0 || h.x + dd[(int)eDirection].x >= sizeMapX ||
            h.y + dd[(int)eDirection].y < 0 || h.y + dd[(int)eDirection].y >= sizeMapY)
            return null;
        return hexagonMap[h.x + dd[(int)eDirection].x][h.y + dd[(int)eDirection].y];
    }

    [HideInInspector]
    public GameObject tileSelected = null;

    public LayerMask test;

    [HideInInspector]
    public bool isOverUI = false;

    [HideInInspector]
    public int numberOfZ = 0;
    [HideInInspector]
    public int totalPop = 0;
    [HideInInspector]
    public int numberOfInf = 0;

    // Update is called once per frame
    void Update()
    {
        if (GameOver < 0.1f)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UI.QuitPanel.SetActive(!UI.QuitPanel.activeSelf);
            if (UI.QuitPanel.activeSelf)
            {
                Pause = 0;
            }
            else
            {
                Pause = 1;
            }
            UI.PausedImage.gameObject.SetActive(Pause == 0 ? true : false);
        }

        if (UI.QuitPanel.activeSelf)
        {
            return;
        }

        UI.StopperBuyButton.interactable = Cash >= 5;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Pause = Pause == 1 ? 0 : 1;
            UI.PausedImage.gameObject.SetActive(Pause == 0 ? true : false);
        }

        Accel = UI.SliderSpeed.value;

        // Raycast
        if (Input.GetMouseButtonDown(0) && isOverUI == false)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;

                Infos infos = objectHit.GetComponent<Infos>();
                tileSelected = objectHit.gameObject;

                UI.PopHeader.SetActive(true);
                UI.Pop.SetActive(true);

                if (infos.type == Infos.buildingType.None)
                {
                    UI.BuildingsPanel.SetActive(true);
                    UI.CasernPanel.SetActive(false);
                    UI.HospitalPanel.SetActive(false);
                    UI.CasernBuyButton.interactable = true;
                    UI.HospitalBuyButton.interactable = true;
                    UI.StopperBuyButton.interactable = true;

                    if (Cash < 50)
                    {
                        UI.CasernBuyButton.interactable = false;
                        UI.HospitalBuyButton.interactable = false;
                    }


                }
                else if (infos.type == Infos.buildingType.Caserne)
                {
                    UI.BuildingsPanel.SetActive(false);
                    UI.CasernPanel.SetActive(true);
                    UI.HospitalPanel.SetActive(false);
                    UI.SoldierBuyButton.interactable = true;
                    if (tileSelected.GetComponent<Infos>().numberOfEntities >= 10 ||
                        Cash < 10)
                    {
                        UI.SoldierBuyButton.interactable = false;
                    }
                }
                else if (infos.type == Infos.buildingType.Hospital)
                {
                    UI.BuildingsPanel.SetActive(false);
                    UI.CasernPanel.SetActive(false);
                    UI.HospitalPanel.SetActive(true);
                    UI.MedicBuyButton.interactable = true;
                    if (tileSelected.GetComponent<Infos>().numberOfEntities >= 10 ||
                        Cash < 10)
                    {
                        UI.MedicBuyButton.interactable = false;
                    }
                }
                else if (infos.type == Infos.buildingType.Stopper)
                {
                    UI.BuildingsPanel.SetActive(false);
                    UI.CasernPanel.SetActive(false);
                    UI.HospitalPanel.SetActive(false);
                    UI.MedicBuyButton.interactable = false;

                    UI.StopperBuyButton.interactable = false;

                }
                else
                {
                    UI.BuildingsPanel.SetActive(false);
                    UI.CasernPanel.SetActive(false);
                    UI.HospitalPanel.SetActive(false);
                    UI.StopperBuyButton.interactable = false;
                }

                //List<GameObject> characters = GetComponent<CharacterCreator>().medicInGame;

                //foreach(GameObject character in characters)
                //{
                //    character.GetComponent<CharacterControl>().GoTo(objectHit.gameObject.GetComponent<Hexagon>());
                //}
                //if (objectHit.gameObject.GetComponent<Hexagon>())
                //{
                //    objectHit.gameObject.GetComponent<Hexagon>().Weight = 0;
                //    DestroyImmediate(objectHit.gameObject);
                //}
            }
        }

        if (tileSelected)
        {
            Infos info = tileSelected.GetComponent<Infos>();

            UI.LivingsValueText.text = "" + info.LivingPopulation;
            UI.InfectedsValueText.text = "" + info.InfectedPopulation;
            UI.UndeadsValueText.text = "" + info.UndeadPopulation;
            UI.DeadsValueText.text = "" + info.DeadPopulation;

            UI.SoldierRankText.text = "" + info.numberOfEntities;
            UI.MedicRankText.text = "" + info.numberOfEntities;

        }

        UI.MoneyText.text = "" + Cash;

        UI.CasernBuyButton.interactable = true;
        UI.HospitalBuyButton.interactable = true;
        UI.SoldierBuyButton.interactable = true;
        UI.MedicBuyButton.interactable = true;

        if (Cash < 10)
        {
            UI.SoldierBuyButton.interactable = false;
            UI.MedicBuyButton.interactable = false;
        }

        if (Cash < 50)
        {
            UI.CasernBuyButton.interactable = false;
            UI.HospitalBuyButton.interactable = false;
        }

        if (Cash > 500)
        {
            Cash = 500;
        }
    }



    public void LateUpdate()
    {
        if (numberOfInf == 0 && numberOfZ == 0 && im.daysPassed > 5)
        {
            GameOver = 0.0f;
            UI.WonPanel.SetActive(true);
        }

        if ((float)numberOfZ / (float)totalPop > 0.9f)
        {
            GameOver = 0.0f;
            UI.GameOverPanel.SetActive(true);
        }

        numberOfZ = 0;
        totalPop = 0;
        numberOfInf = 0;
    }

    public void Heal()
    {
        buildings.ForEach(b =>
        {
            if (b.GetComponent<Infos>().type == Infos.buildingType.Hospital)
            {
                b.GetComponent<Infos>().life -= b.GetComponent<Infos>().UndeadPopulation / 5;
                if (b.GetComponent<Infos>().life <= 0)
                {
                    sfx.PlayerOnShot(2);
                    buildings.Remove(b);
                    Destroy(b.GetComponent<Infos>().lifeBar.transform.parent.gameObject);
                    Destroy(b.GetComponent<Infos>().model);
                    Destroy(b.transform.GetComponentInChildren<ParticleSystem>().gameObject);
                    b.GetComponent<Infos>().Init();
                    b.GetComponent<Hexagon>().building = null;

                }
                else
                {
                    Cash += 3;
                    b.GetComponent<Infos>().inZoneBuildings.ForEach(iz =>
                    {
                        Infos inf = iz.GetComponent<Infos>();
                        int nbrOfHealed = b.GetComponent<Infos>().numberOfEntities * 3;

                        if (inf.InfectedPopulation - nbrOfHealed < 0)
                        {
                            nbrOfHealed = nbrOfHealed - Mathf.Abs(inf.InfectedPopulation - nbrOfHealed);
                        }
                        inf.InfectedPopulation -= nbrOfHealed;
                        inf.DisinfectedPopulation += nbrOfHealed;
                    });
                }
            }
            else if (b.GetComponent<Infos>().type == Infos.buildingType.Caserne)
            {
                b.GetComponent<Infos>().life -= b.GetComponent<Infos>().InfectedPopulation / 10;
                if (b.GetComponent<Infos>().life <= 0)
                {
                    sfx.PlayerOnShot(2);
                    buildings.Remove(b);
                    Destroy(b.GetComponent<Infos>().lifeBar.transform.parent.gameObject);
                    Destroy(b.GetComponent<Infos>().model);
                    Destroy(b.transform.GetComponentInChildren<ParticleSystem>().gameObject);
                    b.GetComponent<Hexagon>().building = null;
                    b.GetComponent<Infos>().Init();
                }
                else
                {
                    Cash += 3;
                    b.GetComponent<Infos>().inZoneBuildings.ForEach(iz =>
                    {
                        Infos inf = iz.GetComponent<Infos>();
                        int nbrOfKilled = b.GetComponent<Infos>().numberOfEntities * 1;

                        if (inf.UndeadPopulation - nbrOfKilled < 0)
                        {
                            nbrOfKilled = inf.UndeadPopulation;
                        }
                        inf.UndeadPopulation -= nbrOfKilled;
                        inf.DeadPopulation += nbrOfKilled;
                        inf.LivingPopulation -= nbrOfKilled;
                        if (inf.LivingPopulation < 0)
                            inf.LivingPopulation = 0;
                    });
                }
            }
        });
    }

    // 1.5 2.8



    #region Callbacks

    public void YesQuit()
    {
        sfx.PlayerOnShot(0);
        SceneManager.LoadScene("MenuScene");
    }

    public void NoQuit()
    {
        UI.QuitPanel.SetActive(false);
        sfx.PlayerOnShot(0);
    }

    public void Retry()
    {
        sfx.PlayerOnShot(0);
        SceneManager.LoadScene("BaseScene");
    }

    public void Quit()
    {
        sfx.PlayerOnShot(0);
        SceneManager.LoadScene("MenuScene");
    }

    public void BuyCasern()
    {
        sfx.PlayerOnShot(1);
        Cash -= 50;
        UI.MoneyText.text = "" + Cash;
        Infos infos = tileSelected.GetComponent<Infos>();
        bc.PlaceBuildingOnHexagon(tileSelected.GetComponent<Hexagon>(), BuildingCreator.EBuildingType.Caserne);
        UI.BuildingsPanel.SetActive(false);
        UI.CasernPanel.SetActive(true);
        UI.HospitalPanel.SetActive(false);
        UI.SoldierBuyButton.interactable = true;
        if (tileSelected.GetComponent<Infos>().numberOfEntities >= 10 ||
            Cash < 10)
        {
            UI.SoldierBuyButton.interactable = false;
        }

        infos.numberOfEntities++;
        infos.life += 50;
        infos.maxlife += 50;
        buildings.Add(tileSelected);

        GameObject r = GameObject.Instantiate(smallRing);
        r.transform.parent = tileSelected.transform;
        r.transform.localPosition = Vector3.zero;

        RaycastHit[] hits = Physics.SphereCastAll(tileSelected.transform.position, 1.5f, Vector3.up);
        for (int i = 0; i < hits.Length; ++i)
            tileSelected.GetComponent<Infos>().inZoneBuildings.Add(hits[i].transform.gameObject);

        GameObject lb = GameObject.Instantiate(lifeBar);
        lb.transform.parent = UI.LifeCanvas.transform;
        lb.transform.position = tileSelected.transform.position + new Vector3(-1, 1.2f, -5);
        Rect rect = lb.GetComponent<RectTransform>().rect;
        rect.width = 10;
        rect.height = 5;
        infos.lifeBar = lb.transform.GetChild(1).GetComponent<Image>();
    }

    public void BuyHospital()
    {
        sfx.PlayerOnShot(1);
        Cash -= 50;
        UI.MoneyText.text = "" + Cash;
        Infos infos = tileSelected.GetComponent<Infos>();
        bc.PlaceBuildingOnHexagon(tileSelected.GetComponent<Hexagon>(), BuildingCreator.EBuildingType.Hospital);
        UI.BuildingsPanel.SetActive(false);
        UI.HospitalPanel.SetActive(true);
        UI.CasernPanel.SetActive(false);
        UI.MedicBuyButton.interactable = true;
        if (tileSelected.GetComponent<Infos>().numberOfEntities >= 10 ||
            Cash < 10)
        {
            UI.MedicBuyButton.interactable = false;
        }

        infos.numberOfEntities++;
        infos.life += 50;
        infos.maxlife += 50;
        buildings.Add(tileSelected);

        GameObject r = GameObject.Instantiate(smallRing);
        r.transform.parent = tileSelected.transform;
        r.transform.localPosition = Vector3.zero;

        RaycastHit[] hits = Physics.SphereCastAll(tileSelected.transform.position, 1.5f, Vector3.up);
        for (int i = 0; i < hits.Length; ++i)
            tileSelected.GetComponent<Infos>().inZoneBuildings.Add(hits[i].transform.gameObject);

        GameObject lb = GameObject.Instantiate(lifeBar);
        lb.transform.parent = UI.LifeCanvas.transform;
        lb.transform.position = tileSelected.transform.position + new Vector3(-1, 1.2f, -5);
        Rect rect = lb.GetComponent<RectTransform>().rect;
        rect.width = 10;
        rect.height = 5;
        infos.lifeBar = lb.transform.GetChild(1).GetComponent<Image>();
    }

    public void BuyStopper()
    {
        sfx.PlayerOnShot(1);
        Cash -= 5;
        if (tileSelected == null)
            return;
        UI.MoneyText.text = "" + Cash;
        Infos infos = tileSelected.GetComponent<Infos>();
        bc.PlaceBuildingOnHexagon(tileSelected.GetComponent<Hexagon>(), BuildingCreator.EBuildingType.Stopper);
        UI.BuildingsPanel.SetActive(false);
        UI.HospitalPanel.SetActive(false);
        UI.CasernPanel.SetActive(false);
        UI.StopperBuyButton.interactable = true;
        UI.MedicBuyButton.interactable = false;

        if (tileSelected.GetComponent<Infos>().numberOfEntities >= 10 ||
            Cash < 2)
        {
            UI.StopperBuyButton.interactable = false;
        }
        buildings.Add(tileSelected);
        infos.life += 50;
        infos.maxlife += 50;
        buildings.Add(tileSelected);

        GameObject lb = GameObject.Instantiate(lifeBar);
        lb.transform.parent = UI.LifeCanvas.transform;
        lb.transform.position = tileSelected.transform.position + new Vector3(-1, 1.2f, -5);
        Rect rect = lb.GetComponent<RectTransform>().rect;
        rect.width = 10;
        rect.height = 5;
        infos.lifeBar = lb.transform.GetChild(1).GetComponent<Image>();
    }

    public void BuySoldier()
    {
        Cash -= 10;
        UI.MoneyText.text = "" + Cash;
        ++tileSelected.GetComponent<Infos>().numberOfEntities;
        tileSelected.GetComponent<Infos>().life += 50;
        tileSelected.GetComponent<Infos>().maxlife += 50;

        UI.SoldierRankText.text = "" + tileSelected.GetComponent<Infos>().numberOfEntities;

        UI.SoldierBuyButton.interactable = true;
        if (tileSelected.GetComponent<Infos>().numberOfEntities >= 10)
        {
            UI.SoldierBuyButton.interactable = false;
            tileSelected.GetComponent<Infos>().inZoneBuildings.Clear();
            RaycastHit[] hits = Physics.SphereCastAll(tileSelected.transform.position, 2.8f, Vector3.up);
            for (int i = 0; i < hits.Length; ++i)
                tileSelected.GetComponent<Infos>().inZoneBuildings.Add(hits[i].transform.gameObject);

            var sh = tileSelected.transform.GetComponentInChildren<ParticleSystem>().shape;
            sh.radius = 3.2f;
        }
    }

    public void BuyMedic()
    {
        Cash -= 10;
        UI.MoneyText.text = "" + Cash;
        ++tileSelected.GetComponent<Infos>().numberOfEntities;
        tileSelected.GetComponent<Infos>().life += 50;
        tileSelected.GetComponent<Infos>().maxlife += 50;

        UI.MedicRankText.text = "" + tileSelected.GetComponent<Infos>().numberOfEntities;

        UI.MedicBuyButton.interactable = true;
        if (tileSelected.GetComponent<Infos>().numberOfEntities >= 10)
        {
            UI.MedicBuyButton.interactable = false;
            tileSelected.GetComponent<Infos>().inZoneBuildings.Clear();
            RaycastHit[] hits = Physics.SphereCastAll(tileSelected.transform.position, 2.8f, Vector3.up);
            for (int i = 0; i < hits.Length; ++i)
                tileSelected.GetComponent<Infos>().inZoneBuildings.Add(hits[i].transform.gameObject);

            var sh = tileSelected.transform.GetComponentInChildren<ParticleSystem>().shape;
            sh.radius = 3.2f;
        }
    }

    #endregion

    [HideInInspector]
    public List<GameObject> buildings = new List<GameObject>();

    BuildingCreator bc;
    [HideInInspector]
    public UIManager UI;

    InfectionManager im;

    GameObject smallRing;
    GameObject bigRing;

    GameObject lifeBar;

    GameObject zombie = null;
}