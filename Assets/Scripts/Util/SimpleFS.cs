using UnityEngine;
using System.Collections;

public class SimpleFS {

	public static string ReadFile(string name){
		return System.IO.File.ReadAllText(Application.dataPath+"/"+name);
	}
}
