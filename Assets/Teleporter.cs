using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {

	public enum Direction {Right, Left, Up, Down};
	public Direction TeleportIfMovesTo = Direction.Right;


    public Transform OtherEnd;

    void OnTriggerStay2D(Collider2D other)
    {
        if (OtherEnd != null && other.attachedRigidbody != null) 
		{
			// Teleport works only on rigidBodies...
			// that move to a given direction. (To prevent teleporting back and forth)
			if ( (TeleportIfMovesTo == Direction.Right && other.attachedRigidbody.velocity.x > 0f) ||
			     (TeleportIfMovesTo == Direction.Left && other.attachedRigidbody.velocity.x < 0f) )
			{
				other.gameObject.transform.position = new Vector3( OtherEnd.position.x, other.gameObject.transform.position.y, other.gameObject.transform.position.z );
			}
			if ( (TeleportIfMovesTo == Direction.Up && other.attachedRigidbody.velocity.y > 0f) ||
			     (TeleportIfMovesTo == Direction.Down && other.attachedRigidbody.velocity.y < 0f) )
			{
				other.gameObject.transform.position = new Vector3( other.gameObject.transform.position.x, OtherEnd.position.y, other.gameObject.transform.position.z );
			}
		}
    }

}
