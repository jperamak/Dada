using UnityEngine;
using System.Collections;

public class ParallaxScrolling : MonoBehaviour 
{

	public Transform Maincamera;
	public float scrollFactor = 0.5f;
	public float autoScrollXSpeed = 0.0f;
		
		
		// Update is called once per frame
		//public float scrollSpeed = 0.5F;
		void Update() 
		{
			//	float offset = Time.time * scrollSpeed;
			Vector2 texOffset = new Vector2( Maincamera.position.x * scrollFactor / transform.localScale.x + autoScrollXSpeed * Time.time, 
		                                     Maincamera.position.y * scrollFactor / transform.localScale.y);
			GetComponent<Renderer>().material.SetTextureOffset("_MainTex", texOffset);
		}
	}
