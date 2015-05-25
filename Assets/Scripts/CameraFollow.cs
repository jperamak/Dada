using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{

    public float moveSmooth = 1f;
    public float zoomSmooth = 8f;

    public float buffer = 4f;

    public float minZoom;
    private float maxZoom;

    private Vector2 minPlayer;
    private Vector2 maxPlayer;
    private Vector3 topRight, bottomLeft, average;
    
    [HideInInspector]
    public List<Transform> players;


    void Awake()
    {
        topRight = GameObject.Find("cameraTopRightBound").transform.position;
        bottomLeft = GameObject.Find("cameraBottomLeftBound").transform.position;
        float x = (topRight.x - bottomLeft.x) / 2f * Screen.height / Screen.width;
        float y = (topRight.y - bottomLeft.y) / 2f;
        maxZoom = x > y ? x : y;
    }

    public void AddPlayer(Transform p)
    {
        players.Add(p);
    }

    public void RemovePlayer(Transform p)
    {
        players.Remove(p);
    }


    void LateUpdate()
    {
        FindCorners();
        AdjustZoom();
        Move();
    }

    void Move()
    {
        average = (minPlayer + maxPlayer) / 2;
        float targetX = Mathf.Lerp(transform.position.x, average.x, moveSmooth * Time.deltaTime);
        float targetY = Mathf.Lerp(transform.position.y, average.y, moveSmooth * Time.deltaTime);
        targetX = Mathf.Clamp(targetX, bottomLeft.x + GetComponent<Camera>().orthographicSize * Screen.width / Screen.height, topRight.x - GetComponent<Camera>().orthographicSize * Screen.width / Screen.height);
        targetY = Mathf.Clamp(targetY, bottomLeft.y + GetComponent<Camera>().orthographicSize, topRight.y - GetComponent<Camera>().orthographicSize);
        transform.position = new Vector3(targetX, targetY, transform.position.z);
    }


    void AdjustZoom()
    {
        float x = (maxPlayer.x - minPlayer.x) / 2f * Screen.height / Screen.width;
        float y = (maxPlayer.y - minPlayer.y) / 2f;


        if (x < minZoom && y < minZoom)
            GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, minZoom + buffer, zoomSmooth * Time.deltaTime);
        else
        {
            float zoom = x > y ? x : y;
            zoom = Mathf.Lerp(GetComponent<Camera>().orthographicSize, zoom + buffer, zoomSmooth * Time.deltaTime);
            GetComponent<Camera>().orthographicSize = zoom < maxZoom ? zoom : maxZoom;
        }
    }

    void FindCorners()
    {
        minPlayer = topRight;
        maxPlayer = bottomLeft;
        foreach (Transform p in players)
        {
            if (p.position.x < minPlayer.x)
                minPlayer.x = p.position.x;
            if (p.position.y < minPlayer.y)
                minPlayer.y = p.position.y;

            if (p.position.x > maxPlayer.x)
                maxPlayer.x = p.position.x;
            if (p.position.y > maxPlayer.y)
                maxPlayer.y = p.position.y;

            if (maxPlayer.x > topRight.x - buffer)
                maxPlayer.x = topRight.x - buffer;
            if (maxPlayer.y > topRight.y - buffer)
                maxPlayer.y = topRight.y - buffer;

            if (minPlayer.x < bottomLeft.x + buffer)
                minPlayer.x = bottomLeft.x + buffer;
            if (minPlayer.y < bottomLeft.y + buffer)
                minPlayer.y = bottomLeft.y + buffer;
        }
    }
}
