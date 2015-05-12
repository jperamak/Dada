using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {

	public enum Direction {Right, Left, Up, Down};
	public Direction TeleportIfMovesTo = Direction.Right;


    public Transform OtherEnd;

    void OnTriggerStay2D(Collider2D other)
    {
		// Teleport works only on rigidBodies...
        if (OtherEnd != null && other.attachedRigidbody != null) 
		{
			// ...that move to a given direction. (To prevent teleporting back and forth)
			if ( (TeleportIfMovesTo == Direction.Right && other.attachedRigidbody.velocity.x > 0f) ||
			     (TeleportIfMovesTo == Direction.Left && other.attachedRigidbody.velocity.x < 0f) ||
			     (TeleportIfMovesTo == Direction.Up && other.attachedRigidbody.velocity.y > 0f) ||
			     (TeleportIfMovesTo == Direction.Down && other.attachedRigidbody.velocity.y < 0f) )
			{
			 // move the position of the object to other end of teleport. Would be better if it would take in account 
			 // the thickness of the teleports.
			 other.gameObject.transform.position += OtherEnd.position - transform.position;
			}
		}
    }

}
