#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static partial class D2D_Helper
{
	public static bool BaseRectSet;
	
	public static Rect BaseRect;
	
	private static GUIStyle none;
	
	private static GUIStyle error;
	
	private static GUIStyle noError;
	
	private static string undoName;
	
	public static GUIStyle None
	{
		get
		{
			if (none == null)
			{
				none = new GUIStyle();
			}
			
			return none;
		}
	}
	
	public static GUIStyle Error
	{
		get
		{
			if (error == null)
			{
				error                   = new GUIStyle();
				error.border            = new RectOffset(3, 3, 3, 3);
				error.normal            = new GUIStyleState();
				error.normal.background = CreateTempTexture(12, 12, "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAALElEQVQIHWP4z8CgC8SHgfg/lNZlQBIACYIlGEEMBjTABOQfQRM7AlKGYSYAoOwcvDRV9/MAAAAASUVORK5CYII=");
			}
			
			return error;
		}
	}
	
	public static GUIStyle NoError
	{
		get
		{
			if (noError == null)
			{
				noError        = new GUIStyle();
				noError.border = new RectOffset(3, 3, 3, 3);
				noError.normal = new GUIStyleState();
			}
			
			return noError;
		}
	}
	
	public static void BeginUndo(string newUndoName)
	{
		undoName = newUndoName;
	}
	
	public static void UpdateUndo(Object o)
	{
		if (o != null)
		{
			Undo.RecordObject(o, undoName);
		}
	}
	
	public static Texture2D CreateTempTexture(int width, int height, string encoded)
	{
		var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
		
		texture.hideFlags = HideFlags.HideAndDontSave;
		texture.LoadImage(System.Convert.FromBase64String(encoded));
		texture.Apply();
		
		return texture;
	}
	
	public static Rect Reserve(float height = 16.0f)
	{
		var rect = default(Rect);
		
		EditorGUILayout.BeginVertical(NoError);
		{
			rect = EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.LabelField(string.Empty, GUILayout.Height(height));
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndVertical();
		
		if (BaseRectSet == true)
		{
			rect.xMin = BaseRect.xMin;
			rect.xMax = BaseRect.xMax;
		}
		
		return rect;
	}
	
	public static void Record<T>(T t, string desc)
		where T : Object
	{
		if (t != null)
		{
			Undo.RecordObject(t, desc);
		}
	}
	
	public static void Record<T>(T[] ts, string desc)
		where T : Object
	{
		if (ts != null)
		{
			Undo.RecordObjects(ts, desc);
		}
	}
	
	public static void SetDirty<T>(T t)
		where T : Object
	{
		if (t != null)
		{
			EditorUtility.SetDirty(t);
		}
	}
	
	public static void SetDirty<T>(T[] ts)
		where T : Object
	{
		foreach (var t in ts)
		{
			SetDirty(t);
		}
	}
	
	public static List<T> LoadAllAssets<T>(string pattern) // e.g. "*.prefab"
		where T : Object
	{
		var assets   = new List<T>();
		var basePath = Application.dataPath;
		var files    = new List<string>(); GetFilesRecursive(files, basePath, pattern);
		var sub      = basePath.Length - "Assets".Length;
		
		for (var i = 0; i < files.Count; i++)
		{
			EditorUtility.DisplayProgressBar("Loading Assets", "", (float)files.Count / (float)i);
			
			var file  = files[i];
			var path  = file.Substring(sub);
			var asset = LoadAsset<T>(path);
			
			if (asset != null)
			{
				assets.Add(asset);
			}
		}
		
		EditorUtility.ClearProgressBar();
		
		return assets;
	}
	
	public static T LoadAsset<T>(string path)
		where T : Object
	{
		return (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
	}
	
	private static void GetFilesRecursive(List<string> files, string path, string pattern)
	{
		files.AddRange(System.IO.Directory.GetFiles(path, pattern));
		
		var directories = System.IO.Directory.GetDirectories(path);
		
		foreach (var directory in directories)
		{
			GetFilesRecursive(files, directory, pattern);
		}
	}
	
	public static T GetAssetImporter<T>(Object asset)
		where T : AssetImporter
	{
		return GetAssetImporter<T>((AssetDatabase.GetAssetPath(asset)));
	}
	
	public static T GetAssetImporter<T>(string path)
		where T : AssetImporter
	{
		return (T)AssetImporter.GetAtPath(path);
	}
	
	public static void ReimportAsset(Object asset)
	{
		ReimportAsset(AssetDatabase.GetAssetPath(asset));
	}
	
	public static void ReimportAsset(string path)
	{
		AssetDatabase.ImportAsset(path);
	}
	
	public static void MakeTextureReadable(Texture2D texture)
	{
		if (texture != null)
		{
			var importer = D2D_Helper.GetAssetImporter<UnityEditor.TextureImporter>(texture);
			
			if (importer != null && importer.isReadable == false)
			{
				importer.isReadable = true;
				
				D2D_Helper.ReimportAsset(importer.assetPath);
			}
		}
	}
	
	public static void SelectAndPing(Object o)
	{
		Selection.activeObject = o;
		
		EditorApplication.delayCall += () => EditorGUIUtility.PingObject(o);
	}
}
#endif