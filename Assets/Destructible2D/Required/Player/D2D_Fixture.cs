using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Fixture")]
public class D2D_Fixture : MonoBehaviour
{
	public static List<D2D_Fixture> AllFixtures = new List<D2D_Fixture>();
	
	public Vector3 Offset;
	
	public Component Target;
	
	[D2D_RangeAttribute(0.01f, 1.0f)]
	public float Threshold = 0.5f;
	
	public bool Pinned = true;
	
	private D2D_Destructible destructible;
	
	private bool dirty = true;
	
	[SerializeField]
	private int fixtureID;
	
	private static int nextFixtureID = 1;
	
	protected virtual void OnDestructibleSplit(D2D_SplitData splitData)
	{
		// Assign a fixtureID to the parent, this will be copied to the clones
		if (splitData.IsClone == false)
		{
			if (nextFixtureID > 1000000)
			{
				nextFixtureID = 1;
			}
			else
			{
				nextFixtureID += 1;
			}
			
			fixtureID = nextFixtureID;
		}
		
		dirty = true;
	}
	
	protected virtual void OnAlphaTexReplaced()
	{
		dirty = true;
	}
	
	protected virtual void OnAlphaTexModified()
	{
		dirty = true;
	}
	
	protected virtual void OnEnable()
	{
		AllFixtures.Add(this);
	}
	
	protected virtual void OnDisable()
	{
		AllFixtures.Remove(this);
	}
	
	protected virtual void Update()
	{
		if (dirty == true)
		{
			dirty        = false;
			destructible = D2D_Helper.GetComponentUpwards<D2D_Destructible>(transform);
			
			if (destructible != null)
			{
				var alpha = destructible.GetAlpha(transform.TransformPoint(Offset));
				
				// Break fixture?
				if (alpha < Threshold)
				{
					DestroyFixture();
				}
				// Break others?
				else if (fixtureID > 0)
				{
					for (var i = AllFixtures.Count - 1; i >= 0; i--)
					{
						var fixture = AllFixtures[i];
						
						if (fixture != null && fixture != this && fixture.fixtureID == fixtureID)
						{
							fixture.DestroyFixture();
						}
					}
				}
			}
		}
	}
	
#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color  = Color.red;
		
		Gizmos.DrawLine(Offset - Vector3.left, Offset + Vector3.left);
		Gizmos.DrawLine(Offset - Vector3.up  , Offset + Vector3.up  );
	}
#endif
	
	public static D2D_Fixture FindFixture(string name, Transform transform)
	{
		if (transform != null)
		{
			var destructible = transform.GetComponentInParent<D2D_Destructible>();
			
			if (destructible != null)
			{
				var fixtures = destructible.GetComponentsInChildren<D2D_Fixture>();
				
				foreach (var fixture in fixtures)
				{
					if (fixture.name == name)
					{
						return fixture;
					}
				}
			}
		}
		
		return null;
	}
	
	private void DestroyFixture()
	{
		if (Target != null)
		{
			D2D_Helper.Destroy(Target);
			D2D_Helper.Destroy(this);
		}
		else
		{
			D2D_Helper.Destroy(gameObject);
		}
	}
}