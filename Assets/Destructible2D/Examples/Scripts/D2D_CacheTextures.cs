using UnityEngine;
using System.Collections.Generic;

// Reading/writing textures for the first time can cause lag, so this script can be used to do it on scene load
[AddComponentMenu("Destructible 2D/D2D Cache Textures")]
public class D2D_CacheTextures : MonoBehaviour
{
	public List<Texture2D> ReadableTextures = new List<Texture2D>();
	
	protected virtual void Awake()
	{
		foreach (var readableTexture in ReadableTextures)
		{
			if (readableTexture != null && readableTexture.width > 0 && readableTexture.height > 0)
			{
				readableTexture.GetPixel(0, 0);
			}
		}
	}
}