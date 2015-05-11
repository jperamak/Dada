using UnityEngine;
using System.Collections;

public class GlueChildren : MonoBehaviour {
	
	//public GameObject[] gluedObjects;
	
	void Start () {

		if (transform.childCount != 2)
			Debug.LogError("GlueChildren needs to have exactly two children");

		CreateHingeBetween( transform.GetChild(0).gameObject, transform.GetChild(1).gameObject);
	}
	
	void CreateHingeBetween(GameObject gobj1, GameObject gobj2) {
		HingeJoint2D hj = gobj2.AddComponent<HingeJoint2D>();
		
		// Connect hinge to the other (glued) object
		if(gobj1.GetComponent<Rigidbody2D>() != null)
			hj.connectedBody = gobj1.GetComponent<Rigidbody2D>();
		else
			Debug.LogError("Tried to glue a GameObject without RigidBody2D");
		
		// Limit the joint so it cannot rotate
		hj.useLimits = true;
		hj.limits = new JointAngleLimits2D();
		
		// Set anchor point to other objects center
		hj.anchor = gobj2.transform.InverseTransformPoint( gobj1.transform.position );
	}
	
	
	
}
