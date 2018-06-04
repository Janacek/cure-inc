using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Range(0, 1)]
    public float CameraSpanSpeed = 0.5f;
    [Range(0, 1)]
    public float CameraScrollSpeed = 0.5f;

    void Start()
    {

    }

    

    public void ZoomOnHexagon(Hexagon hex)
    {
        Vector3 oldPos = transform.position;
        transform.position = new Vector3(hex.transform.position.x, hex.transform.position.y, oldPos.z);
    }
    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            clickPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 dir = clickPos - Input.mousePosition;

            Camera.main.transform.position += dir * CameraSpanSpeed;

            clickPos = Input.mousePosition;
        }

        Camera.main.orthographicSize += Input.mouseScrollDelta.y * CameraScrollSpeed;
        if (Camera.main.orthographicSize < 3)
        {
            Camera.main.orthographicSize = 3;
        }
        if (Camera.main.orthographicSize > 40)
        {
            Camera.main.orthographicSize = 40;
        }

    }

    Vector3 clickPos;
}
