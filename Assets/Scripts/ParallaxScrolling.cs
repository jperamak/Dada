using UnityEngine;
using System.Collections;

public class ParallaxScrolling : MonoBehaviour {

	public Camera camera;
	public float scrollFactor = 1.0f;

	// Use this for initialization
	void Start () {

	}
	
	
	// Update is called once per frame
	//public float scrollSpeed = 0.5F;
	void Update() {
		//	float offset = Time.time * scrollSpeed;
		GetComponent<Renderer>().material.SetTextureOffset("_MainTex", 
		                                                   new Vector2( camera.transform.position.x/100.0F * scrollFactor, 
		            									   camera.transform.position.y/70.0F * scrollFactor));
	}
}

