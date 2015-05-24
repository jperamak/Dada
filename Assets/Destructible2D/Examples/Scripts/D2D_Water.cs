using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Water")]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class D2D_Water : MonoBehaviour
{
	public Color WaveColour = Color.blue;
	
	public int WaveCount = 10;
	
	public float WaveWidth = 1.0f;
	
	public float WaveThickness = 1.0f;
	
	public float WaveAmplitude = 1.0f;
	
	public float WaveFrequency = 1.0f;
	
	public float WaveOffset;
	
	public float WaveAge;
	
	public float WaveSpeed = 0.1f;
	
	public Color SeaColour = Color.black;
	
	public float SeaDepth = 10.0f;
	
	public Texture2D NoiseTex;
	
	[HideInInspector]
	[SerializeField]
	private MeshFilter meshFilter;
	
	private Mesh mesh;
	
	private Vector3[] positions;
	
	private Color[] colours;
	
	private Vector2[] uvs;
	
	private int[] indices;
	
	protected virtual void Update()
	{
		if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();
		
		if (mesh == null)
		{
			mesh = new Mesh();
			mesh.hideFlags = HideFlags.DontSave;
		}
		
		// Prevent this from dirtying the scene when exiting play mode
		D2D_Helper.StealthSet(meshFilter, mesh);
		
		// Generate positions?
		if (positions == null || positions.Length != WaveCount * 3 + 3)
		{
			positions = new Vector3[WaveCount * 3 + 3];
		}
		
		// Generate colours?
		if (colours == null || colours.Length != WaveCount * 3 + 3)
		{
			colours = new Color[WaveCount * 3 + 3];
		}
		
		for (var i = 0; i <= WaveCount; i++)
		{
			colours[i * 3 + 0] = WaveColour;
			colours[i * 3 + 1] = SeaColour;
			colours[i * 3 + 2] = SeaColour;
		}
		
		// Generate uvs?
		if (uvs == null || uvs.Length != WaveCount * 3 + 3)
		{
			uvs = new Vector2[WaveCount * 3 + 3];
		}
		
		// Generate indices?
		if (indices == null || indices.Length != WaveCount * 12)
		{
			indices = new int[WaveCount * 12];
			
			for (var i = 0; i < WaveCount; i++)
			{
				// Wave
				indices[i * 12 + 0] = i * 3 + 0;
				indices[i * 12 + 1] = i * 3 + 1;
				indices[i * 12 + 2] = i * 3 + 3;
				indices[i * 12 + 3] = i * 3 + 4;
				indices[i * 12 + 4] = i * 3 + 3;
				indices[i * 12 + 5] = i * 3 + 1;
				
				// Sea
				indices[i * 12 +  6] = i * 3 + 1;
				indices[i * 12 +  7] = i * 3 + 2;
				indices[i * 12 +  8] = i * 3 + 4;
				indices[i * 12 +  9] = i * 3 + 5;
				indices[i * 12 + 10] = i * 3 + 4;
				indices[i * 12 + 11] = i * 3 + 2;
			}
		}
		
		// Make waves move?
		if (NoiseTex != null)
		{
			WaveAge += WaveSpeed * Time.deltaTime;
			
			var halfSize = WaveCount * WaveWidth * 0.5f;
			
			for (var i = 0; i <= WaveCount; i++)
			{
				var sample = NoiseTex.GetPixelBilinear(WaveOffset + i * WaveFrequency, WaveAge).r - 0.5f;
				var x      = i * WaveWidth - halfSize;
				var y      = sample * 2.0f * WaveAmplitude;
				
				positions[i * 3 + 0] = new Vector3(x, y                , 0.0f);
				positions[i * 3 + 1] = new Vector3(x, y - WaveThickness, 0.0f);
				positions[i * 3 + 2] = new Vector3(x, y - SeaDepth     , 0.0f);
			}
		}
		
		// Update mesh
		mesh.Clear();
		mesh.vertices  = positions;
		mesh.colors    = colours;
		mesh.triangles = indices;
		mesh.uv        = uvs;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}
	
	protected virtual void OnDestroy()
	{
		D2D_Helper.Destroy(mesh);
	}
}