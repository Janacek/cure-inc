using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    protected List<Hexagon> Path = new List<Hexagon>();
    public Vector2 localOffsetBegin;
    public Vector2 localOffsetEnd;
    public HexagonManager HexagonManager;
    public float Speed = 4.0f;
    protected Hexagon CurrentHexagon;
    protected Vector3 Goal;
    protected Vector3 Direction;
    protected Pathfinding pathfinding = new Pathfinding();

    public void Start()
    {
        hm = GameObject.Find("HexagonManager").GetComponent<HexagonManager>();
    }

    public virtual void OnCurrentHexagonChanged(Hexagon newHexagon)
    {
        CurrentHexagon = newHexagon;
    }
    // Update is called once per frame
    void Update ()
    {
        if (Path.Count == 0)
        {
            return;
        }
        Vector3 dirToGoal = Goal - transform.position;
        dirToGoal.Normalize();
        float dot = Vector3.Dot(Direction, dirToGoal);
        if (dot < 0)
        {
            Path.RemoveAt(0);
            if (Path.Count > 1)
            {
                OnCurrentHexagonChanged(Path[0]);
                Vector2 pos = new Vector2();
                pos.x = Path[1].transform.position.x;//Random.Range(Path[0].transform.position.x + localOffsetBegin.x, Path[0].transform.position.x + localOffsetEnd.x);
                pos.y = Path[1].transform.position.y;// Random.Range(Path[0].transform.position.y + localOffsetBegin.y, Path[0].transform.position.y + localOffsetEnd.y);
                Goal = pos;
                Direction = Goal - transform.position;
                Direction.Normalize();
            }
        }
        transform.position += (Vector3)Direction * Speed * Time.deltaTime * hm.Pause;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach(Hexagon hex in Path)
        {
            //Gizmos.DrawSphere(hex.gameObject.transform.position, 0.5f);
        }
        //Gizmos.DrawSphere(Goal, 0.5f);

    }

    public void GoTo(Hexagon hexagonGoal)
    {
        Path = pathfinding.ComputePathBetweenHexagons(CurrentHexagon, hexagonGoal, HexagonManager);
        if (Path.Count <= 1)
        {
            return;
        }
        Vector2 pos = new Vector2();
        pos.x = Path[1].transform.position.x;// Random.Range(Path[0].transform.position.x + localOffsetBegin.x, Path[0].transform.position.x + localOffsetEnd.x);
        pos.y = Path[1].transform.position.y; //Random.Range(Path[0].transform.position.y + localOffsetBegin.y, Path[0].transform.position.y + localOffsetEnd.y);

        Goal = pos;
        Direction = Goal - transform.position;
        Direction.Normalize();
    }

    HexagonManager hm;
}
