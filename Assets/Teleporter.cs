using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {

    public Transform OtherEnd;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (OtherEnd != null) 
		{
			 // Transform the entering object to the other end of the teleport 
			 other.gameObject.transform.position += OtherEnd.position - transform.position;
		}
    }

}
