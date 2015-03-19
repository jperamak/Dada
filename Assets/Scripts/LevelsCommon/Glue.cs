using UnityEngine;
using System.Collections;


// A helper component that attaches objects with RigidBody2D together by creating 
// a non-rotating hinge.

public class Glue : MonoBehaviour {

	public GameObject[] gluedObjects;

	void Start () {

		foreach ( GameObject gobj in gluedObjects) {
			if (gobj != null) 
				CreateHingeWith(gobj);
		}
	}

	void CreateHingeWith(GameObject gobj) {
		HingeJoint2D hj = gameObject.AddComponent<HingeJoint2D>();
		
		// Connect hinge to the other (glued) object
		if(gobj.GetComponent<Rigidbody2D>() != null)
			hj.connectedBody = gobj.GetComponent<Rigidbody2D>();
		else
			Debug.LogError("Tried to glue a GameObject without RigidBody2D");
		
		// Limit the joint so it cannot rotate
		hj.useLimits = true;
		hj.limits = new JointAngleLimits2D();
		
		// Set anchor point to other objects center
		hj.anchor = transform.InverseTransformPoint( gobj.transform.position );
	}



}
