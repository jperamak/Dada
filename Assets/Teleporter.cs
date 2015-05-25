using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {

	public enum Direction {Right, Left, Up, Down};
	public Direction TeleportIfMovesTo = Direction.Right;


    public Transform OtherEnd = null;
	public GameObject TeleportsTo = null;

	Teleporter otherTeleport = null;

	void Start() {

	}


    void OnTriggerStay2D(Collider2D other)
    {

		if (TeleportsTo != null && IsObjectMovingRightWay( other.attachedRigidbody )) 
		{
			Teleporter otherTeleport = TeleportsTo.GetComponent<Teleporter>();
			if (otherTeleport == null)
				Debug.LogError("Receiving end is not a Teleporter");

			float distFromAxis = GetDistanceFromCentralAxis( other.attachedRigidbody );
			other.transform.position = otherTeleport.GetExitPosition( distFromAxis );
		}
    }


	bool IsObjectMovingRightWay( Rigidbody2D rigidBody ) {
		return ( (TeleportIfMovesTo == Direction.Right && rigidBody.velocity.x > 0f) ||
		         (TeleportIfMovesTo == Direction.Left  && rigidBody.velocity.x < 0f) ||
		         (TeleportIfMovesTo == Direction.Up    && rigidBody.velocity.y > 0f) ||
		         (TeleportIfMovesTo == Direction.Down  && rigidBody.velocity.y < 0f) );
	}

	float GetDistanceFromCentralAxis( Rigidbody2D rigidBody ) {
		if (TeleportIfMovesTo == Direction.Right || TeleportIfMovesTo == Direction.Left)
			return rigidBody.transform.position.y - transform.position.y;
		else
			return rigidBody.transform.position.x - transform.position.x;
	}

	Vector3 GetExitPosition( float distCentralAxis ) {
		if (TeleportIfMovesTo == Direction.Right || TeleportIfMovesTo == Direction.Left)
			return new Vector3(transform.position.x, transform.position.y + distCentralAxis, 0f);
		return new Vector3(transform.position.x + distCentralAxis, transform.position.y, 0f);
	}




}
