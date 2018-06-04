using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    public int x;
    public int y;
    public float Weight = 1;

    [HideInInspector]
    public bool checkedOn = false;



    LineRenderer[] lines = new LineRenderer[6];

    public void ConstructHexagon(Vector2 Pos, Material material, GameObject linePrefab)
    {
        transform.position = Pos;

        Mesh hexMesh = new Mesh();
        Vector3[] vertices = new Vector3[7];
        Vector3[] verticesLines = new Vector3[7];
        int[] tri = new int[18];
        Vector3[] normals = new Vector3[6];
        Vector2[] uvs = new Vector2[7];

        vertices[0] = Vector2.zero;
        verticesLines[0] = Vector2.zero;
        int triInd = 0;
        for (int i = 0; i < 6; ++i)
        {
            Vector2 hexCorner = ComputeHexCorner(new Vector2(0, 0), 1f, i);
            Vector2 hexCornerLine = ComputeHexCorner(new Vector2(0, 0), 0.95f, i);
            vertices[i + 1] = hexCorner;
            verticesLines[i + 1] = hexCornerLine;

            tri[triInd] = 0;
            tri[triInd + 1] = (i + 1) % 6 + 1;
            tri[triInd + 2] = i + 1;

            normals[i] = Vector3.up;


            triInd += 3;

        }

        uvs[0] = new Vector2(0.5f, 0.5f);

        uvs[1] = new Vector2(0.5f, 0);
        uvs[2] = new Vector2(1, 0.25f);
        uvs[3] = new Vector2(1, 0.75f);

        uvs[4] = new Vector2(0.5f, 1);
        uvs[5] = new Vector2(0, 0.75f);
        uvs[6] = new Vector2(0, 0.25f);

        hexMesh.vertices = vertices;
        hexMesh.triangles = tri;
        hexMesh.uv = uvs;
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        filter.mesh = hexMesh;
        renderer.material = material;

        MeshCollider collider = gameObject.AddComponent<MeshCollider>();
        collider.sharedMesh = null;
        collider.sharedMesh = hexMesh;

        for (int i = 0; i < 6; ++i)
        {
            Vector2 pos1 = verticesLines[(i + 1) % 6 + 1];
            Vector2 pos2 = verticesLines[i + 1];

            GameObject l = GameObject.Instantiate(linePrefab);
            LineRenderer lr = l.GetComponent<LineRenderer>();
            l.transform.parent = transform;
            lr.SetPosition(0, transform.position + new Vector3(pos1.x, pos1.y, 0));
            lr.SetPosition(1, transform.position + new Vector3(pos2.x, pos2.y, 0));
            lr.material = new Material(Shader.Find("Standard"));

            if (i == 0)
            {
                lines[i] = lr;
            }
            else
            {
                lines[6 - i] = lr;
            }
        }

        //lines[2].sortingOrder = 30;
        //lines[3].sortingOrder = 30;

        //lines[1].sortingOrder = 20;
        //lines[4].sortingOrder = 20;

        //lines[0].sortingOrder = 10;
        //lines[5].sortingOrder = 10;


        float zoneType = Mathf.PerlinNoise(transform.position.x / 10, transform.position.y / 10) * 20;
        int roundedType = (int)zoneType;
        renderer.material.SetFloat("_Height", zoneType / 20);
        renderer.material.SetColor("_Color", HexagonManager.Colors[roundedType]);
        baseColor = HexagonManager.Colors[roundedType];

        gameObject.AddComponent<Infos>();
        infos = GetComponent<Infos>();

        infos.tType = HexagonManager.TileTypes[roundedType];
        if (infos.tType == Infos.tileType.Mountain || infos.tType == Infos.tileType.Water)
            Weight = 0;
        else
            Weight = 1;

    }

    private Vector2 ComputeHexCorner(Vector2 hexCenter, float size, int i)
    {
        float fAngleDeg = 60f * i + 30f;
        float fAngleRad = Mathf.PI / 180f * fAngleDeg;
        return new Vector2(hexCenter.x + size * Mathf.Cos(fAngleRad), hexCenter.y + size * Mathf.Sin(fAngleRad));
    }

    private void Update()
    {
        renderer.material.SetColor("_Color", Color.Lerp(baseColor, Color.red, infos.InfectionQuotient));

        //lines[2].material.SetColor("_Color", Color.red);

        for (int i = 0; i < (int)HexagonManager.EDirectionHexagon.eCount; ++i)
        {
            GameObject h = hm.GetNeighbor(gameObject, (HexagonManager.EDirectionHexagon)i);
            if (h && h.GetComponent<Infos>().InfectionQuotient <= 0.0f &&
                infos.InfectionQuotient > 0.0f)
            {
                lines[i].gameObject.SetActive(true);
            }
            else if (h == null && infos.InfectionQuotient > 0.0f)
            {
                lines[i].gameObject.SetActive(true);
            }
            else
            {
                lines[i].gameObject.SetActive(false);
            }
        }

        if (infos.UndeadPopulation > 0)
        {
            Zombie.SetActive(true);
        }
        else
        {
            Zombie.SetActive(false);
        }
    }


    public GameObject building = null;
    Color baseColor;
    MeshRenderer renderer;
    Infos infos;
    [HideInInspector]
    public HexagonManager hm = null;
    [HideInInspector]
    public GameObject Zombie = null;
}
