using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour 
{
	public float xMargin = 1f;		// Distance in the x axis the player can move before the camera follows.
	public float yMargin = 1f;		// Distance in the y axis the player can move before the camera follows.
	public float xSmooth = 8f;		// How smoothly the camera catches up with it's target movement in the x axis.
	public float ySmooth = 8f;		// How smoothly the camera catches up with it's target movement in the y axis.
	public Vector2 maxXAndY;		// The maximum x and y coordinates the camera can have.
	public Vector2 minXAndY;		// The minimum x and y coordinates the camera can have.
    public float maxZoom;
    public float minZoom;


    public Vector2 average;
    private Vector2 minPlayer;
    private Vector2 maxPlayer;
    private float vertExtent;
    private float horExtent;

	public List<Transform> players;		// Reference to the player's transform.


	void Awake ()
	{
		// Setting up the reference.
        average = Vector2.zero;
        minPlayer = Vector2.zero;
        maxPlayer = Vector2.zero;

	}

    public void AddPlayer(Transform p)
    {
        players.Add(p);
    }

    public void RemovePlayer(Transform p)
    {
        players.Remove(p);
    }

	bool CheckXMargin()
	{
		// Returns true if the distance between the camera and the player in the x axis is greater than the x margin.
        foreach (Transform t in players)
            if (Mathf.Abs(transform.position.x - t.position.x) > xMargin)
                return true;
        return false;
	}


	bool CheckYMargin()
	{
		// Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
        foreach (Transform t in players)
            if (Mathf.Abs(transform.position.y - t.position.y) > yMargin)
                return true;
        return false;
	}


	void LateUpdate ()
	{
        if (players.Count > 0)
		    TrackPlayer();
	}
	
    void AdjustZoom()
    {
        float x = Mathf.Abs(minPlayer.x - maxPlayer.x);
        float y = Mathf.Abs(minPlayer.y - maxPlayer.y);


        if (x < minZoom && y < minZoom)
            GetComponent<Camera>().orthographicSize = minZoom;
        else //if (x > camera.orthographicSize || y > camera.orthographicSize)
        {
            GetComponent<Camera>().orthographicSize = x > y ? x : y;
            GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize < maxZoom ? GetComponent<Camera>().orthographicSize : maxZoom;
        }
        vertExtent = Camera.main.orthographicSize;
        horExtent = Camera.main.orthographicSize * Screen.width / Screen.height;

    }
	
	void TrackPlayer ()
	{
        minPlayer = maxXAndY;
        maxPlayer = minXAndY;
        foreach (Transform t in players)
        {
            minPlayer.x = minPlayer.x < t.position.x ? minPlayer.x : t.position.x;
            minPlayer.y = minPlayer.y < t.position.y ? minPlayer.y : t.position.y;

            maxPlayer.x = maxPlayer.x > t.position.x ? maxPlayer.x : t.position.x;
            maxPlayer.y = maxPlayer.y > t.position.y ? maxPlayer.y : t.position.y;

            average = (minPlayer + maxPlayer) * 0.5f;
        }



		// By default the target x and y coordinates of the camera are it's current x and y coordinates.
		float targetX = transform.position.x;
		float targetY = transform.position.y;

        AdjustZoom();

		// If the player has moved beyond the x margin...
		if(CheckXMargin())
			// ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
            targetX = Mathf.Lerp(transform.position.x, average.x, xSmooth * Time.deltaTime);

		// If the player has moved beyond the y margin...
		if(CheckYMargin())
			// ... the target y coordinate should be a Lerp between the camera's current y position and the player's current y position.
            targetY = Mathf.Lerp(transform.position.y, average.y, ySmooth * Time.deltaTime);

		// The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
        targetX = Mathf.Clamp(targetX, horExtent + minXAndY.x * 0.5f , maxXAndY.x * 0.5f - horExtent);
        targetY = Mathf.Clamp(targetY, vertExtent +  minXAndY.y * 0.5f, maxXAndY.y * 0.5f - vertExtent);

		// Set the camera's position to the target position with the same z component.
		transform.position = new Vector3(targetX, targetY, transform.position.z);
	}
}
