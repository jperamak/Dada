using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public static partial class D2D_Helper
{
	public static int MeshVertexLimit = 65000;
	
	public const string ComponentMenuPrefix = "Destructible 2D/D2D ";
	
	public static void Swap<T>(ref T a, ref T b)
	{
		var c = b;
		
		b = a;
		a = c;
	}
	
	public static float Atan2(Vector2 xy)
	{
		return Mathf.Atan2(xy.x, xy.y);
	}
	
	public static void ResizeArrayTo<T>(List<T> array, int size, System.Func<int, T> newT, System.Action<T> removeT)
	{
		if (array != null)
		{
			while (array.Count < size)
			{
				array.Add(newT != null ? newT(array.Count) : default(T));
			}
			
			while (array.Count > size)
			{
				if (removeT != null)
				{
					removeT(array[array.Count - 1]);
				}
				
				array.RemoveAt(array.Count - 1);
			}
		}
	}
	
	private static Object stealthSetObject;
	
	private static HideFlags stealthSetFlags;
	
	public static void BeginStealthSet(Object o)
	{
		if (o != null)
		{
			stealthSetObject = o;
			stealthSetFlags  = o.hideFlags;
			
			o.hideFlags = HideFlags.DontSave;
		}
	}
	
	public static void EndStealthSet()
	{
		if (stealthSetObject != null)
		{
			stealthSetObject.hideFlags = stealthSetFlags;
		}
	}
	
	public static void StealthSet(MeshFilter mf, Mesh m)
	{
		if (mf != null && mf.sharedMesh != m)
		{
#if UNITY_EDITOR
			var hf = mf.hideFlags;
			
			mf.hideFlags  = HideFlags.DontSave;
			mf.sharedMesh = m;
			mf.hideFlags  = hf;
#else
			mf.sharedMesh = m;
#endif
		}
	}
	
	public static void StealthSet(MeshRenderer mr, Material m)
	{
		if (mr != null && mr.sharedMaterial != m)
		{
#if UNITY_EDITOR
			var hf = mr.hideFlags;
			
			mr.hideFlags      = HideFlags.DontSave;
			mr.sharedMaterial = m;
			mr.hideFlags      = hf;
#else
			mr.sharedMaterial = m;
#endif
		}
	}
	
	public static void SetPosition(Transform t, Vector3 v)
	{
		if (t != null)
		{
#if UNITY_EDITOR
			if (Application.isPlaying == false && t.position == v) return;
#endif
			t.position = v;
		}
	}
	
	public static T Destroy<T>(T o)
		where T : Object
	{
		if (o != null)
		{
#if UNITY_EDITOR
			if (Application.isPlaying == false)
			{
				Object.DestroyImmediate(o, true); return null;
			}
#endif
			Object.Destroy(o);
		}
		
		return null;
	}
	
	public static void DestroyManaged(System.Action DestroyAction)
	{
		if (DestroyAction != null)
		{
#if UNITY_EDITOR
			var isPlaying = Application.isPlaying;
		
			EditorApplication.delayCall += () =>
				{
					if (Application.isPlaying == isPlaying)
					{
						DestroyAction();
					}
				};
#else
			DestroyAction();
#endif
		}
	}
	
	public static void SetParent(Transform t, Transform newParent, bool keepLocalTransform = true)
	{
		if (t != null && t.parent != newParent)
		{
			if (keepLocalTransform == true)
			{
				var oldLocalPosition = t.localPosition;
				var oldLocalRotation = t.localRotation;
				var oldLocalScale    = t.localScale;
				
				t.parent        = newParent;
				t.localPosition = oldLocalPosition;
				t.localRotation = oldLocalRotation;
				t.localScale    = oldLocalScale;
			}
			else
			{
				t.parent = newParent;
			}
		}
	}
	
	public static void SetLocalPosition(Transform t, float x, float y, float z)
	{
		SetLocalPosition(t, new Vector3(x, y, z));
	}
	
	public static void SetLocalPosition(Transform t, Vector3 v)
	{
		if (t != null)
		{
#if UNITY_EDITOR
			if (Application.isPlaying == false && t.localPosition == v) return;
#endif
			t.localPosition = v;
		}
	}
	
	public static void SetLocalScale(Transform t, float v)
	{
		SetLocalScale(t, new Vector3(v, v, v));
	}
	
	public static void SetLocalScale(Transform t, Vector3 v)
	{
		if (t != null)
		{
#if UNITY_EDITOR
			if (Application.isPlaying == false && t.localScale == v) return;
#endif
			if (t.localScale == v) return;
			
			t.localScale = v;
		}
	}
	
	public static T GetComponentUpwards<T>(Transform transform, bool skipFirst = false)
		where T : Component
	{
		if (transform != null)
		{
			if (skipFirst == true)
			{
				transform = transform.parent;
			}
			
			while (transform != null)
			{
				var component = transform.GetComponent<T>();
				
				if (component != null) return component;
				
				transform = transform.parent;
			}
		}
		
		return null;
	}
	
	public static T GetOrAddComponent<T>(GameObject gameObject)
		where T : Component
	{
		if (gameObject != null)
		{
			var component = gameObject.GetComponent<T>();
			
			if (component == null) component = gameObject.AddComponent<T>();
			
			return component;
		}
		
		return null;
	}
	
	public static GameObject CloneGameObject(GameObject source, Transform parent, bool keepName = false)
	{
		if (source != null)
		{
			var clone = (GameObject)GameObject.Instantiate(source); if (clone == null) throw new System.NullReferenceException();
			
			if (parent   != null) SetParent(clone.transform, parent, true);
			if (keepName == true) clone.name = source.name;
			
			return clone;
		}
		
		return source;
	}
	
	public static GameObject CloneGameObject(GameObject source, Transform parent, Vector3 xyz, Quaternion rot, bool keepName = false)
	{
		if (source != null)
		{
			var clone = (GameObject)GameObject.Instantiate(source, xyz, rot); if (clone == null) throw new System.NullReferenceException();
			
			if (parent   != null) SetParent(clone.transform, parent, true);
			if (keepName == true) clone.name = source.name;
			
			return clone;
		}
		
		return source;
	}
	
	public static GameObject CreateGameObject(string name = "", Transform parent = null, bool recordUndo = false)
	{
		return CreateGameObject(name, parent, Vector3.zero, Quaternion.identity, Vector3.one, recordUndo);
	}
	
	public static GameObject CreateGameObject(string name, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, bool recordUndo = false)
	{
		var gameObject = new GameObject(name);
		
		gameObject.transform.parent        = parent;
		gameObject.transform.localPosition = localPosition;
		gameObject.transform.localRotation = localRotation;
		gameObject.transform.localScale    = localScale;
		
#if UNITY_EDITOR
		if (recordUndo == true)
		{
			Undo.RegisterCreatedObjectUndo(gameObject, undoName);
		}
#endif
		
		return gameObject;
	}
	
	public static T Clone<T>(T o, bool keepName = true)
		where T : Object
	{
		if (o != null)
		{
			var c = (T)Object.Instantiate(o);
			
			if (c != null && keepName == true) c.name = o.name;
			
			return c;
		}
		
		return null;
	}
	
	public static void SendMessage(Component component, string messageName, SendMessageOptions smo)
	{
		SendMessage(component, messageName, smo);
	}
	
	public static void SendMessage(Component component, string messageName, object o, SendMessageOptions smo)
	{
		if (component != null)
		{
#if UNITY_EDITOR
			if (Application.isPlaying == false)
			{
				var cs = component.GetComponents<Component>();
				
				foreach (var c in cs)
				{
					var method = c.GetType().GetMethod(messageName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
					
					if (method != null)
					{
						if (o != null)
						{
							method.Invoke(c, new object[] { method });
						}
						else
						{
							method.Invoke(c, null);
						}
					}
				}
				
				return;
			}
#endif
			component.SendMessage(messageName, o, smo);
		}
	}
	
	public static void BroadcastMessage(Transform transform, string messageName, SendMessageOptions smo)
	{
		BroadcastMessage(transform, messageName, null, smo);
	}
	
	public static void BroadcastMessage(Transform transform, string messageName, object o, SendMessageOptions smo)
	{
		if (transform != null)
		{
			SendMessage(transform, messageName, o, smo);
			
			for (var i = transform.childCount - 1; i >= 0; i--)
			{
				BroadcastMessage(transform.GetChild(i), messageName, o, smo);
			}
		}
	}
	
	public static bool Enabled(Behaviour b)
	{
		return b != null && b.enabled == true && b.gameObject.activeInHierarchy == true;
	}
	
	public static float Divide(float a, float b)
	{
		return Zero(b) == false ? a / b : 0.0f;
	}
	
	public static Vector2 Divide(float xA, float yA, float xB, float yB)
	{
		return new Vector2(Divide(xA, xB), Divide(yA, yB));
	}
	
	public static float Reciprocal(float v)
	{
		return Zero(v) == false ? 1.0f / v : 0.0f;
	}
	
	public static Vector2 Reciprocal(Vector2 v)
	{
		return new Vector2(Reciprocal(v.x), Reciprocal(v.y));
	}
	
	public static Vector2 Reciprocal(float x, float y)
	{
		return new Vector2(Reciprocal(x), Reciprocal(y));
	}
	
	public static Vector3 Reciprocal(Vector3 v)
	{
		return new Vector3(Reciprocal(v.x), Reciprocal(v.y), Reciprocal(v.z));
	}
	
	public static Vector3 Reciprocal(float x, float y, float z)
	{
		return new Vector3(Reciprocal(x), Reciprocal(y), Reciprocal(z));
	}
	
	public static bool Zero(float v)
	{
		return v == 0.0f;
		//return Mathf.Approximately(v, 0.0f);
	}
	
	public static Matrix4x4 RotationMatrix(Quaternion q)
	{
		var matrix = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
		
		return matrix;
	}
	
	public static Matrix4x4 TranslationMatrix(Vector3 xyz)
	{
		return TranslationMatrix(xyz.x, xyz.y, xyz.z);
	}
	
	public static Matrix4x4 TranslationMatrix(float x, float y, float z)
	{
		var matrix = Matrix4x4.identity;
		
		matrix.m03 = x;
		matrix.m13 = y;
		matrix.m23 = z;
		
		return matrix;
	}
	
	public static Matrix4x4 ScalingMatrix(float xyz)
	{
		return ScalingMatrix(xyz, xyz, xyz);
	}
	
	public static Matrix4x4 ScalingMatrix(Vector3 xyz)
	{
		return ScalingMatrix(xyz.x, xyz.y, xyz.z);
	}
	
	public static Matrix4x4 ScalingMatrix(float x, float y, float z)
	{
		var matrix = Matrix4x4.identity;
		
		matrix.m00 = x;
		matrix.m11 = y;
		matrix.m22 = z;
		
		return matrix;
	}
	
	public static float DampenFactor(float dampening, float elapsed)
	{
		return 1.0f - Mathf.Pow((float)System.Math.E, - dampening * elapsed);
	}
	
	public static Quaternion Dampen(Quaternion current, Quaternion target, float dampening, float elapsed, float minStep = 0.0f)
	{
		var factor   = DampenFactor(dampening, elapsed);
		var maxDelta = Quaternion.Angle(current, target) * factor + minStep * elapsed;
		
		return MoveTowards(current, target, maxDelta);
	}
	
	public static float Dampen(float current, float target, float dampening, float elapsed, float minStep = 0.0f)
	{
		var factor   = DampenFactor(dampening, elapsed);
		var maxDelta = Mathf.Abs(target - current) * factor + minStep * elapsed;
		
		return MoveTowards(current, target, maxDelta);
	}
	
	public static Vector3 Dampen3(Vector3 current, Vector3 target, float dampening, float elapsed, float minStep = 0.0f)
	{
		var factor   = DampenFactor(dampening, elapsed);
		var maxDelta = Mathf.Abs((target - current).magnitude) * factor + minStep * elapsed;
		
		return Vector3.MoveTowards(current, target, maxDelta);
	}
	
	public static Quaternion MoveTowards(Quaternion current, Quaternion target, float maxDelta)
	{
		var delta = Quaternion.Angle(current, target);
		
		return Quaternion.Slerp(current, target, Divide(maxDelta, delta));
	}
	
	public static float MoveTowards(float current, float target, float maxDelta)
	{
		if (target > current)
		{
			current = System.Math.Min(target, current + maxDelta);
		}
		else
		{
			current = System.Math.Max(target, current - maxDelta);
		}
		
		return current;
	}
	
	public static Vector3 ClosestPointToLineSegment(Vector3 a, Vector3 b, Vector3 point)
	{
		var l = (b - a).magnitude;
		var d = (b - a).normalized;
		
		return a + Mathf.Clamp(Vector3.Dot(point - a, d), 0.0f, l) * d;
	}
	
	public static Vector3 ClosestPointToTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 p)
	{
		var r  = Quaternion.Inverse(Quaternion.LookRotation(-Vector3.Cross(a - b, a - c)));
		var ra = r * a;
		var rb = r * b;
		var rc = r * c;
		var rp = r * p;
		
		var a2 = D2D_Helper.VectorXY(ra);
		var b2 = D2D_Helper.VectorXY(rb);
		var c2 = D2D_Helper.VectorXY(rc);
		var p2 = D2D_Helper.VectorXY(rp);
		
		if (PointLeftOfLine(a2, b2, p2) == true)
		{
			return ClosestPointToLineSegment(a, b, p);
		}
		
		if (PointLeftOfLine(b2, c2, p2) == true)
		{
			return ClosestPointToLineSegment(b, c, p);
		}
		
		if (PointLeftOfLine(c2, a2, p2) == true)
		{
			return ClosestPointToLineSegment(c, a, p);
		}
		
		var barycentric = GetBarycentric(a2, b2, c2, p2);
		
		return barycentric.x * a + barycentric.y * b + barycentric.z * c;
	}
	
	public static Vector3 GetBarycentric(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
	{
		var barycentric = Vector3.zero;
		var v0          = b - a;
		var v1          = c - a;
		var v2          = p - a;
		var d00         = Vector2.Dot(v0, v0);
		var d01         = Vector2.Dot(v0, v1);
		var d11         = Vector2.Dot(v1, v1);
		var d20         = Vector2.Dot(v2, v0);
		var d21         = Vector2.Dot(v2, v1);
		var denom       = D2D_Helper.Reciprocal(d00 * d11 - d01 * d01);
		
		barycentric.y = (d11 * d20 - d01 * d21) * denom;
		barycentric.z = (d00 * d21 - d01 * d20) * denom;
		barycentric.x = 1.0f - barycentric.y - barycentric.z;
		
		return barycentric;
	}
	
	public static bool PointLeftOfLine(Vector2 a, Vector2 b, Vector2 p) // NOTE: CCW
	{
		return ((b.x - a.x) * (p.y - a.y) - (p.x - a.x) * (b.y - a.y)) >= 0.0f;
	}
	
	public static bool PointRightOfLine(Vector2 a, Vector2 b, Vector2 p) // NOTE: CCW
	{
		return ((b.x - a.x) * (p.y - a.y) - (p.x - a.x) * (b.y - a.y)) <= 0.0f;
	}
	
	public static Vector2 VectorXY(Vector3 xyz)
	{
		return new Vector2(xyz.x, xyz.y);
	}
	
	public static byte[] ExtractAlphaData(Texture2D texture)
	{
		if (texture != null)
		{
#if UNITY_EDITOR
			D2D_Helper.MakeTextureReadable(texture);
#endif
			var width  = texture.width;
			var height = texture.height;
			var total  = width * height;
			var data   = new byte[total];
			
			for (var y = 0; y < height; y++)
			{
				for (var x = 0; x < width; x++)
				{
					data[x + y * width] = (byte)(texture.GetPixel(x, y).a * 255.0f);
				}
			}
			
			return data;
		}
		
		return null;
	}
	
	public static bool ExtractAlphaData(Sprite sprite, ref byte[] data, ref int width, ref int height)
	{
		if (sprite != null && sprite.texture != null)
		{
#if UNITY_EDITOR
			D2D_Helper.MakeTextureReadable(sprite.texture);
#endif
			var rect         = sprite.textureRect;
			var sourceWidth  = sprite.texture.width;
			var sourcePixels = sprite.texture.GetPixels32();
			var sourceOffset = sourceWidth * Mathf.CeilToInt(rect.y) + Mathf.CeilToInt(rect.x);
			var targetOffset = 0;
			
			width  = Mathf.FloorToInt(rect.width);
			height = Mathf.FloorToInt(rect.height);
			
			var total = width * height;
			
			if (data == null || data.Length != total)
			{
				data = new byte[width * height];
			}
			
			for (var y = 0; y < height; y++)
			{
				for (var x = 0; x < width; x++)
				{
					data[targetOffset + x] = sourcePixels[sourceOffset + x].a;
				}
				
				sourceOffset += sourceWidth;
				targetOffset += width;
			}
			
			return true;
		}
		
		return false;
	}
}